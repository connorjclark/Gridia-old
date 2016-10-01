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
        public ContainerView Toolbar;

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

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TabView.IsActive.Value = !TabView.IsActive.Value;
                //TabView.UpdateView();
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
                Inventory.SetValue("Items", items);
                Toolbar.SetValue("Items", new ObservableList<ItemInstance>(items.GetRange(0, NumItemsInToolbar)));
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

            }
        }

        #endregion Methods
    }
}