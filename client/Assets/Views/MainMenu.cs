namespace MarkLight.UnityProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gridia;

    using MarkLight.ValueConverters;
    using MarkLight.Views;
    using MarkLight.Views.UI;

    using UnityEngine;

    public class MainMenu : UIView
    {
        #region Fields

        public String CurrentServerName;

        // ServerSelect
        public Views.UI.Button HostServerButton;
        public String LoginPassword = "";

        // Login
        public String LoginUsername = "";

        // InputConnectionDetails
        public String ManualServerAddress = "";
        public String ManualServerPort = "";
        public String RegistrationPassword = "";

        // Registration
        public String RegistrationUsername = "";

        // ServerDetails
        public ServerDetails SelectedServer;

        // ServerTitleScreen
        public String ServerChangelog = "";
        public String ServerDescription;
        public ObservableList<ServerDetails> Servers;
        public Views.UI.List ServerSelectList;
        public Views.UI.Panel ServerSelectPanel;
        public ViewSwitcher ViewSwitcher;

        private float _previousServerSelectListWidth;

        #endregion Fields

        #region Methods

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

        public void DisconnectAndEnterEnterServerSelect()
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Close();
            EnterServerSelect();
        }

        public void EnterInputConnectionDetails()
        {
            ViewSwitcher.SwitchTo("InputConnectionDetails");
        }

        public void EnterLogin()
        {
            ViewSwitcher.SwitchTo("Login");
        }

        public void EnterRegistration()
        {
            ViewSwitcher.SwitchTo("Registration");
        }

        public void EnterServerDetails(ServerDetails serverDetails)
        {
            ViewSwitcher.SwitchTo("ServerDetails");
            SelectedServer = serverDetails;
            SetValue(() => CurrentServerName, serverDetails.Name);
            SetValue(() => ServerDescription, serverDetails.Description);
        }

        public void EnterServerSelect()
        {
            ViewSwitcher.SwitchTo("ServerSelect");

            // TODO how to clear ServerSelectList selection?
            // this doesn't work...
            Servers.SelectedItem = null;
            Servers.SelectedIndex = -1;
        }

        public void EnterServerTitleScreen()
        {
            ViewSwitcher.SwitchTo("ServerTitleScreen");
        }

        public void HostServer()
        {
            LocalServer.LaunchServerProcess();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                MainThreadQueue.Instantiate();
                GridiaConstants.InitializeGuiStuff();
                GridiaConstants.InitializeSoundPlayer();
            }

            // var cursorTexture = Resources.Load<Texture2D>("GUI Components/cursorHand_grey");
            // Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

            Servers = new ObservableList<ServerDetails>();
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
                server.Name = "hotengames.com";
                server.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
                server.PlayersOnline = 5;
                server.Address = "www.hotengames.com";
                server.Port = 1044;
                Servers.Add(server);
            }

            HostServerButton.IsActive.Value = LocalServer.CanHostLocally();
        }

        public void ListSelectionChanged(ItemSelectionActionData data)
        {
            if (data.Item != null)
            {
                ServerDetails serverDetails = (ServerDetails)data.Item;
                EnterServerDetails(serverDetails);
            }
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

        public void Update()
        {
            float serverSelectPanelWidth = ServerSelectPanel.ActualWidth - ServerSelectPanel.VerticalScrollbar.ActualWidth;

            if (serverSelectPanelWidth != _previousServerSelectListWidth)
            {
                //ServerSelectList.SetValue(() => ServerSelectList.PresentedListItems[0].Width.Value.Pixels, new ElementSize(serverSelectPanelWidth, ElementSizeUnit.Pixels));
                //ServerSelectList.UpdateLayout();
                _previousServerSelectListWidth = serverSelectPanelWidth;
            }

            // TODO remove this from update loop
            if (GridiaConstants.ErrorMessage == null && ServerConnect.connected && ServerConnect.connecting)
            {
                ServerConnect.connecting = false;
                EnterServerTitleScreen();
            }
        }

        #endregion Methods
    }

    public class ServerDetails
    {
        #region Fields

        public String Address;
        public String Description;
        public String Name;
        public int PlayersOnline;
        public int Port;

        #endregion Fields
    }
}