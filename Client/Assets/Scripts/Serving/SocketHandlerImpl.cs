using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace Serving
{
    public sealed class SocketHandlerImpl : SocketHandler
    {
        private Socket _socket;
        private JavaBinaryReader _in;
        private JavaBinaryWriter _out;

        public SocketHandlerImpl(String host, int port)
        {
            Connect(host, port);
        }

        public void Start(Action onConnectionSettled, SocketHandler topLevelSocketHandler)
        {
            onConnectionSettled();
            while (true)
            {
                HandleData(topLevelSocketHandler);
            }
        }

        public void Send(Message message)
        {
            try
            {
                lock (_out)
                {
                    _out.Write(message.Data.Length);
                    _out.WriteJavaUTF(message.Type);
                    _out.Write(message.Compressed);
                    _out.Write(message.Data);
                }
            }
            catch (IOException ex)
            {
                Close();
            }
        }

        public void Close()
        {
            try
            {
                _out.Close();
                _in.Close();
                _socket.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error closing streams " + ex);
            }
        }

        private void Connect(String host, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(host, port);
            var stream = new NetworkStream(_socket);
            _in = new JavaBinaryReader(stream);
            _out = new JavaBinaryWriter(stream);
        }

        private void HandleData(SocketHandler topLevelSocketHandler)
        {
            int dataSize = _in.ReadInt32();
            var type = _in.ReadJavaUTF();
            var compressed = _in.ReadBoolean();
            var bytes = _in.ReadBytes(dataSize);

            if (compressed)
            {
                bytes = new Decompressor().Uncompress(bytes);
            }

            try
            {
                var handlerType = MessageHandler<SocketHandler, Object>.Get(type);
                var method = handlerType.GetMethod("Handle");
                var ins = Activator.CreateInstance(handlerType);
                method.Invoke(ins, new Object[] { topLevelSocketHandler, bytes });
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception("No such protocol: " + type);
            }
        }

        public JavaBinaryWriter GetOutputStream()
        {
            return _out;
        }

        public JavaBinaryReader GetInputStream()
        {
            return _in;
        }
    }
}
