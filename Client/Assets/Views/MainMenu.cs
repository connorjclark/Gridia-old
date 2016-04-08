using Gridia;
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MarkUX.UnityProject
{
    
    public class ServerDetails
    {
        public String Name;
        public String Address;
        public int Port;
        public String Description;
        public int PlayersOnline;
    }

    public class MainMenu : View
    {
        public ViewSwitcher ViewSwitcher;
        public List<ServerDetails> Servers;

        // ServerSelect
        public Views.Button HostServerButton;
        public Views.Panel ServerSelectPanel;
        public Views.List ServerSelectList;

        // InputConnectionDetails
        public String ManualServerAddress = "";
        public String ManualServerPort = "";

        // ServerDetails
        public ServerDetails SelectedServer;
        public String CurrentServerName;
        public String ServerDescription;

        // ServerTitleScreen
        public String ServerChangelog = "";

        // Login
        public String LoginUsername = "";
        public String LoginPassword = "";

        // Registration
        public String RegistrationUsername = "";
        public String RegistrationPassword = "";

        private float _previousServerSelectListWidth;

        public override void Initialize()
        {
            base.Initialize();
            
            MainThreadQueue.Instantiate();
            GridiaConstants.InitializeGuiStuff();
            GridiaConstants.InitializeSoundPlayer();
            
            var cursorTexture = Resources.Load<Texture2D>("GUI Components/cursorHand_grey");
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

            Servers = new List<ServerDetails>();
            {
                var server = new ServerDetails();
                server.Name = "Local";
                server.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
                server.PlayersOnline = 5;
                server.Address = "localhost";
                server.Port = 1044;
                Servers.Add(server);
            }

            {
                var server = new ServerDetails();
                server.Name = "hotengame.com";
                server.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
                server.PlayersOnline = 5;
                server.Address = "www.hotengames.com";
                server.Port = 1044;
                Servers.Add(server);
            }

            HostServerButton.Enabled = LocalServer.CanHostLocally();
        }

        public void EnterServerSelect()
        {
            ViewSwitcher.SwitchTo("ServerSelect");
            // TODO how to clear ServerSelectList selection?
        }

        public void DisconnectAndEnterEnterServerSelect()
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Close();
            EnterServerSelect();
        }

        public void EnterInputConnectionDetails()
        {
            ViewSwitcher.SwitchTo("InputConnectionDetails");
        }

        public void EnterServerDetails(ServerDetails serverDetails)
        {
            ViewSwitcher.SwitchTo("ServerDetails");
            SelectedServer = serverDetails;
            SetValue(() => CurrentServerName, serverDetails.Name);
            SetValue(() => ServerDescription, serverDetails.Description);
        }

        public void EnterServerTitleScreen()
        {
            ViewSwitcher.SwitchTo("ServerTitleScreen");
        }

        public void EnterLogin()
        {
            ViewSwitcher.SwitchTo("Login");
        }

        public void EnterRegistration()
        {
            ViewSwitcher.SwitchTo("Registration");
        }

        /*public void Connect(String address, int port)
        {
            ServerConnect.Connect(address, port, (bool success, String message) => {
                if (success)
                {
                    ServerConnect.connected = false;
                    EnterServerTitleScreen();
                }
                else
                {
                    // TODO handle bad connect
                }
            });
        }*/

        public void Login()
        {
            ServerConnect.SendLoginRequest(LoginUsername, LoginPassword, (bool success, String message) => {
                if (success)
                {
                    SceneManager.LoadScene("Main");
                }
                else
                {
                    // TODO handle bad login request
                }
            });
        }

        public void Register()
        {
            ServerConnect.SendRegistrationRequest(RegistrationUsername, RegistrationPassword, (bool success, String message) => {
                if (success)
                {
                    SceneManager.LoadScene("Main");
                }
                else
                {
                    // TODO handle bad registration request
                }
            });
        }

        public void ConnectManually()
        {
            int port = 0;
            if (Int32.TryParse(ManualServerPort, out port))
            {
                ServerConnect.Connect(ManualServerAddress, port);
            }
        }

        public void ConnectToSelectedServer()
        {
            ServerConnect.Connect(SelectedServer.Address, SelectedServer.Port);
        }


        public void HostServer()
        {
            LocalServer.LaunchServerProcess();
        }

        public void ListSelectionChanged(ListSelectionActionData data)
        {
            if (data.ListItem != null)
            {
                ServerDetails serverDetails = (ServerDetails)data.ListItem.Item;
                EnterServerDetails(serverDetails);
            }
        }

        public void Update()
        {
            float serverSelectPanelWidth = ServerSelectPanel.ActualWidth - ServerSelectPanel.VerticalScrollBar.ActualWidth;

            if (serverSelectPanelWidth != _previousServerSelectListWidth)
            {
                ServerSelectList.SetValue(() => ServerSelectList.ItemWidth, new ElementSize(serverSelectPanelWidth, ElementSizeUnit.Pixels));
                ServerSelectList.UpdateLayout();
                _previousServerSelectListWidth = serverSelectPanelWidth;
            }

            // TODO remove this from update loop
            if (GridiaConstants.ErrorMessage == null && ServerConnect.connected && ServerConnect.connecting)
            {
                ServerConnect.connecting = false;
                EnterServerTitleScreen();
            }
        }
    }
}
