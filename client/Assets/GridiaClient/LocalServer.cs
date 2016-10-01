namespace Gridia
{
    using System;
    using System.Net.Sockets;
    using System.Threading;

    using UnityEngine;

    public class LocalServer
    {
        #region Methods

        public static bool CanHostLocally()
        {
            return true;
        }

        public static void LaunchServerProcess()
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo {FileName = "java"};
            if (Application.isEditor)
            {
                processInfo.Arguments = "-jar target/server.jar";
                processInfo.WorkingDirectory = "../Server/GridiaServer/";
            }
            else
            {
                processInfo.Arguments = "-jar server.jar";
            }

            var proc = new System.Diagnostics.Process {StartInfo = processInfo};
            proc.Start();
        }

        #endregion Methods
    }
}