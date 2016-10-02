namespace MarkLight.UnityProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Gridia;

    using MarkLight;
    using MarkLight.UnityProject;
    using MarkLight.ValueConverters;
    using MarkLight.Views;
    using MarkLight.Views.UI;

    using UnityEditor.SceneManagement;

    using UnityEngine;

    public class Main : UIView
    {
        #region Fields

        public static Main Instance;

        public ContainerView Inventory;
        public int NumItemsInToolbar = 10;
        public Views.UI.Region TabView;
        public Views.UI.Group ContainerGroup;
        public ContainerView Toolbar;

        private int _maxContainerViewsAllowed = 3;
        private List<ContainerView> ContainerViews = new List<ContainerView>();
        private bool _testMode;

        #endregion Fields

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            _testMode = EditorSceneManager.GetActiveScene().name == "TestMainUI";
            GameState.Create();

            GameState.Instance.ContainerCreated = OnContainerCreated;
            GameState.Instance.ContainerChanged = OnContainerChanged;

            if (_testMode)
            {
                TestInitialize();
            }

            Instance = this;
        }

        public void ShowTabView()
        {
            TabView.IsActive.Value = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TabView.IsActive.Value = !TabView.IsActive.Value;
            }

            if (_testMode)
            {
                TestUpdate();
            }
        }

        private void OnContainerChanged(int containerId, ItemInstance itemInstance, int indexChanged)
        {
            if (containerId == GameState.Instance.InventoryContainerId && indexChanged < NumItemsInToolbar)
            {
                Toolbar.Items[indexChanged] = itemInstance;
            }
        }

        private void OnContainerCreated(int containerId, ObservableList<ItemInstance> items)
        {
            if (containerId == GameState.Instance.InventoryContainerId)
            {
                Inventory.SetItems(items);
                Toolbar.SetItems(new ObservableList<ItemInstance>(items.GetRange(0, NumItemsInToolbar)));
            }
            else if (containerId == GameState.Instance.EquipmentContainerId)
            {
                // TODO
            }
            else
            {
                ContainerView containerView = ContainerViews.Find(cv => cv.ContainerId == containerId);

                if (!containerView)
                {
                    containerView = ContainerGroup.CreateView<ContainerView>();
                    containerView.Alignment.Value = ElementAlignment.Right;
                    containerView.ContainerName = "Container " + containerId;
                    containerView.SetItems(items);
                    containerView.InitializeViews();

                    ContainerViews.Add(containerView);
                }
                else
                {
                    containerView.SetItems(items);
                }

                if (ContainerViews.Count() > _maxContainerViewsAllowed)
                {
                    ContainerViews[0].Destroy();
                    ContainerViews.RemoveAt(0);
                }
            }
        }

        private void TestInitialize()
        {
            if (Application.isPlaying)
            {
                MainThreadQueue.Instantiate();
            }

            Locator.Provide(new ContentManager("demo-world"));
            Locator.Provide(new TextureManager("demo-world"));

            var cm = Locator.Get<ContentManager>();

            var items = new List<ItemInstance>();
            for (int i = 0; i < 50; i++)
            {
                items.Add(new ItemInstance(cm.GetItem(i)));
            }

            GameState.Instance.InventoryContainerId = 0;
            GameState.Instance.SetContainerItems(GameState.Instance.InventoryContainerId, items);
        }

        private void TestUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var items = new List<ItemInstance>();
                var cm = Locator.Get<ContentManager>();
                var r = new System.Random();
                for (int i = 0; i < 50; i++)
                {
                    items.Add(new ItemInstance(cm.GetItem(r.Next(10))));
                }

                GameState.Instance.SetContainerItems(GameState.Instance.InventoryContainerId, items);
            }

            // create new container window
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var items = new List<ItemInstance>();
                var cm = Locator.Get<ContentManager>();
                var r = new System.Random();
                for (int i = 0; i < 20; i++)
                {
                    items.Add(new ItemInstance(cm.GetItem(r.Next(10))));
                }

                GameState.Instance.SetContainerItems(r.Next(), items);
                ShowTabView();
            }
        }

        #endregion Methods
    }
}