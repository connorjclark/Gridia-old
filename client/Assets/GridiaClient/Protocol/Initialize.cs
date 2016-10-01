namespace Gridia.Protocol
{
    using System;

    using Newtonsoft.Json.Linq;

    using Serving;

    class Initialize : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            GridiaConstants.WorldName = (String)data["worldName"];
            GridiaConstants.Size = (int)data["size"];
            GridiaConstants.Depth = (int)data["depth"];
            GridiaConstants.SectorSize = (int)data["sectorSize"];
            GridiaConstants.ServerTimeOffset = connection.getSystemTime() - (long)data["time"];

            if (!GridiaConstants.Version.Equals((String)data["version"]))
            {
                GridiaConstants.ErrorMessage = "Incompatible client. Client version = " + GridiaConstants.Version + ". Server version = " + (String)data["version"] + ". Visit www.hotengames.com for the newest client.";
                GridiaConstants.ErrorMessageAction = connection.Close;
            } else
            {
                //ServerSelection.connectedWaitHandle.Set(); // :(
                ServerConnect.connected = true;
            }
        }

        #endregion Methods
    }
}