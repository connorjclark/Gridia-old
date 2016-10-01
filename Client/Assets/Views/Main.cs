using Gridia;
using MarkLight;
using MarkLight.ValueConverters;
using MarkLight.Views;
using MarkLight.Views.UI;
using MarkLight.UnityProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MarkLight.UnityProject
{
    public class Main : UIView
    {
        public static Main Instance;

        public int NumItemsInToolbar = 10;

        public Views.UI.Region TabView;
        public ContainerView Inventory;
        public ContainerView Toolbar;

        public int InventoryContainerId;
        public Dictionary<int, ObservableList<ItemInstance>> ContainerItemLists = new Dictionary<int, ObservableList<ItemInstance>>(); // :(

        public ObservableList<ItemInstance> GetContainerItems(int containerId)
        {
            if (!ContainerItemLists.ContainsKey(containerId))
            {
                ContainerItemLists[containerId] = new ObservableList<ItemInstance>();

                if (containerId == InventoryContainerId)
                {
                    Inventory.SetValue("Items", ContainerItemLists[containerId]);
                    Toolbar.SetValue("Items", new ObservableList<ItemInstance>());
                }
            }

            return ContainerItemLists[containerId];
        }

        public void SetContainerItems(int containerId, List<ItemInstance> items)
        {
            var containerItems = GetContainerItems(containerId);
            containerItems.Replace(items);
            if (containerId == InventoryContainerId)
            {
                Toolbar.Items.Replace(items.GetRange(0, NumItemsInToolbar));
            }
        }

        public void SetContainerItem(int containerId, ItemInstance itemInstance, int index)
        {
            ContainerItemLists[containerId][index] = itemInstance;
            
            if (containerId == InventoryContainerId && index < NumItemsInToolbar)
            {
                Toolbar.Items[index] = itemInstance;
            }
        }

        private bool _testMode;

        public override void Initialize()
        {
            base.Initialize();

            _testMode = EditorSceneManager.GetActiveScene().name == "TestMainUI";

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

            InventoryContainerId = 0;
            SetContainerItems(InventoryContainerId, items);
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

                SetContainerItems(InventoryContainerId, items);
            }

            // create new container window
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                
            }
        }
    }
}
