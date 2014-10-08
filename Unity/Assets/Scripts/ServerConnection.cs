using System;
using System.Reflection;
using UnityEngine;
namespace Gridia
{
    public class ServerConnection : MonoBehaviour
    {
        public static ServerConnection instance;// major smell

        public static ServerConnection ConnectAsClient(string ip, int port) {
            GameObject go = new GameObject("Server Connection", typeof(NetworkView), typeof(ServerConnection));
            ServerConnection serverConnection = go.GetComponent<ServerConnection>();
            Connect(ip, port);
            return instance = serverConnection;
        }

        public static ServerConnection CreateServer(int port)
        {
            GameObject go = new GameObject("Server", typeof(NetworkView), typeof(ServerConnection));
            ServerConnection serverConnection = go.GetComponent<ServerConnection>();
            Network.InitializeServer(10, port);
            return instance = serverConnection;
        }

        private static void Connect(string ip, int port) {
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                Network.Connect(ip, port);
            }
        }

        public GridiaMain Gridia { get; set; }
        private bool IsServer { get { return Network.peerType == NetworkPeerType.Server; } }
        private NetworkView _view;

        public void Start() {
            _view = GetComponent<NetworkView>();
        }

        public void OnPlayerConnected(NetworkPlayer player)
        {
            MonoBehaviour.print(player);
        }

        private void RPC(string name, RPCMode mode, params object[] args) {
            if (IsServer && mode == RPCMode.Server)
            {
                InvokeForServer(name, args);
            }
            else
            {
                _view.RPC(name, mode, args);
            }
        }

        /*
         *
         * Server RPCs
         * 
         */

        private void InvokeForServer(string name, params object[] args)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++) types[i] = args[i].GetType();
            MethodInfo method = typeof(ServerConnection).GetMethod(name, types);
            method.Invoke(this, args);
        }

        private void RPC_Except(string name, NetworkPlayer except, params object[] args)
        {
            foreach (NetworkPlayer player in Network.connections)
            {
                if (except != player) _view.RPC(name, player, args);
            }
        }

        [RPC]
        public void MovePlayer(NetworkPlayer owner, int x, int y)
        {
            MonoBehaviour.print("got a move request...");
            RPC_Except("MoveCreature", owner, 0, x, y);
        }

        /*
         *
         * Client RPCs
         * 
         */

        [RPC]
        public void MovePlayer(int x, int y)
        {
            MonoBehaviour.print("send a move request...");
            RPC("MovePlayer", RPCMode.Server, _view.owner, x, y);
        }

        [RPC]
        public void MoveCreature(int id, int x, int y)
        {
            MonoBehaviour.print("got: Move! " + x + ", " + y + "|" + id);
            MonoBehaviour.print(Gridia.creatures[id]);
            Gridia.tileMap.UpdateCreature(Gridia.creatures[id], new Vector2(x, y));
        }
    }
}
