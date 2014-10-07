namespace Gridia
{
    public interface IServerConnection  {
        void Connect();
        void MovePlayer(int x, int y);
    }

    public class ServerConnectionMock : IServerConnection {

        private IServer _server;
        private TileMap _tileMap;

        public ServerConnectionMock(TileMap tileMap) {
            _server = new ServerMock();
            _tileMap = tileMap;
        }

        public void Connect()
        {
        }

        public void MovePlayer(int x, int y)
        {
            throw new System.NotImplementedException();
        }
    }
}
