using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class Initialize : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            GridiaConstants.WORLD_NAME = (String)data["worldName"];
            GridiaConstants.SIZE = (int)data["size"];
            GridiaConstants.DEPTH = (int)data["depth"];
            GridiaConstants.SECTOR_SIZE = (int)data["sectorSize"];
            GridiaConstants.SERVER_TIME_OFFSET = connection.getSystemTime() - (long)data["time"];
            if (!GridiaConstants.VERSION.Equals((String)data["version"]))
            {
                GridiaConstants.ErrorMessage = "Incompatible client. Client version = " + GridiaConstants.VERSION + ". Server version = " + (String)data["version"] + ". Visit www.hotengames.com for the newest client.";
                GridiaConstants.ErrorMessageAction = connection.Close;
            } else
            {
                //ServerSelection.connectedWaitHandle.Set(); // :(
                ServerSelection.connected = true;
            }
        }
    }
}
