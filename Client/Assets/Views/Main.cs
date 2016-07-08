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
        public ObservableList<ItemInstance> InventoryItems;
        public ObservableList<ItemInstance> ToolbarItems;
        public int NumItemsInInventory;
        public int NumItemsInToolbar = 10;

        public Views.UI.Region TabView;
        public ContainerView Inventory;
        public ContainerView Toolbar;

        private bool _testMode;

        public override void Initialize()
        {
            base.Initialize();

            _testMode = EditorSceneManager.GetActiveScene().name == "TestMainUI";

            if (_testMode)
            {
                TestInitialize();
            }
        }

        public void SetInventoryItem(ItemInstance itemInstance, int index)
        {
            if (index < NumItemsInToolbar)
            {
                ToolbarItems[index] = itemInstance;
                ToolbarItems.ItemModified(index);
            }

            if (index < NumItemsInInventory)
            {
                InventoryItems[index] = itemInstance;
            }
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

            InventoryItems = new ObservableList<ItemInstance>();
            var cm = Locator.Get<ContentManager>();
            NumItemsInInventory = 50;
            for (int i = 0; i < NumItemsInInventory; i++)
            {
                InventoryItems.Add(new ItemInstance(cm.GetItem(i)));
            }

            ToolbarItems = new ObservableList<ItemInstance>();
            for (int i = 0; i < NumItemsInToolbar; i++)
            {
                ToolbarItems.Add(InventoryItems[i]);
            }
        }

        private void TestUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetInventoryItem(new ItemInstance(Locator.Get<ContentManager>().GetItem(new System.Random().Next(100))), 0);
            }
        }
    }
}
