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

    public delegate void MoveItem(int fromSource, int fromIndex, int toSource, int toIndex);

    public class Main : UIView
    {
        #region Fields

        public static Main Instance;

        public Views.UI.Group ContainerGroup;
        public bool MouseDraggingItemDone = false;
        public ContainerView MouseDownContainer;
        public int MouseDownIndex;
        public ContainerView MouseUpContainer;
        public int MouseUpIndex;
        public int NumItemsInToolbar = 10;
        public Views.UI.Region TabView;
        public ContainerView Toolbar;

        public MoveItem MoveItem;

        private List<ContainerView> ContainerViews = new List<ContainerView>();
        private int _maxContainerViewsAllowed = 3;
        private bool _testMode;

        #endregion Fields

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            _testMode = EditorSceneManager.GetActiveScene().name == "TestMainUI";

            GameState.Instance.ContainerCreated = OnContainerCreated;
            GameState.Instance.ContainerChanged = OnContainerChanged;

            if (_testMode && Application.isPlaying)
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

            if (MouseDraggingItemDone)
            {
                MouseDraggingItemDone = false;

                var fromSource = MouseDownContainer != null ? MouseDownContainer.ContainerId : 0;
                var toSource = MouseUpContainer != null ? MouseUpContainer.ContainerId : 0;

                MoveItem(fromSource, MouseDownIndex, toSource, MouseUpIndex);

                MouseDownContainer = MouseUpContainer = null;
                MouseDownIndex = MouseUpIndex = -1;
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
            String containerName;

            if (containerId == GameState.Instance.InventoryContainerId)
            {
                Toolbar.SetItems(new ObservableList<ItemInstance>(items.GetRange(0, NumItemsInToolbar)));
                Toolbar.ContainerId = containerId;
                containerName = "Inventory";
            }
            else if (containerId == GameState.Instance.EquipmentContainerId)
            {
                containerName = "Equipment";
            }
            else
            {
                containerName = "Container " + containerId;
            }

            ContainerView containerView = ContainerViews.Find(cv => cv.ContainerId == containerId);

            if (!containerView)
            {
                containerView = ContainerGroup.CreateView<ContainerView>();
                containerView.Alignment.Value = ElementAlignment.Right;
                containerView.ContainerName = containerName;
                containerView.SetItems(items);
                containerView.ContainerId = containerId;

                containerView.InitializeViews();

                ContainerViews.Add(containerView);
            }
            else
            {
                containerView.SetItems(items);
            }

            if (ContainerViews.Count() > _maxContainerViewsAllowed)
            {
                // index 0 is the inventory
                ContainerViews[1].Destroy();
                ContainerViews.RemoveAt(1);
            }
        }

        private void TestInitialize()
        {
            Locator.Provide(new ContentManager("demo-world"));
            Locator.Provide(new TextureManager("demo-world"));

            var cm = Locator.Get<ContentManager>();

            var items = new List<ItemInstance>();
            for (int i = 0; i < 25; i++)
            {
                items.Add(new ItemInstance(cm.GetItem(i)));
            }
            for (int i = 0; i < 25; i++)
            {
                items.Add(new ItemInstance(cm.GetItem(0)));
            }

            GameState.Instance.InventoryContainerId = 1;
            GameState.Instance.SetContainerItems(GameState.Instance.InventoryContainerId, items);

            MoveItem = (int fromSource, int fromIndex, int toSource, int toIndex) => {
                if (fromSource > 0 && toSource > 0)
                {
                    var temp = MouseDownContainer.Items[fromIndex];
                    GameState.Instance.SetContainerItem(fromSource, MouseUpContainer.Items[toIndex], fromIndex);
                    GameState.Instance.SetContainerItem(toSource, temp, toIndex);
                }
            };
        }

        private void TestUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var items = new List<ItemInstance>();
                var cm = Locator.Get<ContentManager>();
                var r = new System.Random();
                for (int i = 0; i < 25; i++)
                {
                    items.Add(new ItemInstance(cm.GetItem(r.Next(10))));
                }
                for (int i = 0; i < 25; i++)
                {
                    items.Add(new ItemInstance(cm.GetItem(0)));
                }

                GameState.Instance.SetContainerItems(GameState.Instance.InventoryContainerId, items);
            }

            // create new container
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