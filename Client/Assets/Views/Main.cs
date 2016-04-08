using Gridia;
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MarkUX.UnityProject
{
    // TODO get rid of this
    public class ItemInstanceWrapper
    {
        public static List<ItemInstanceWrapper> GetItemsWithBullshitWrapper(List<ItemInstance> NewItems)
        {
            var Items = new List<ItemInstanceWrapper>();
            
            for (int i = 0; i < NewItems.Count(); i++) 
            {
                Items.Add(new ItemInstanceWrapper());
                Items[i].ItemInstance = NewItems[i];
            }

            return Items;
        }

        public ItemInstance ItemInstance;
    }

    [CreatesView(typeof(ItemView))]
    public class Main : View
    {

        public List<ItemInstanceWrapper> InventoryItems;
        public List<ItemInstanceWrapper> ToolbarItems;
        public int NumItemsInInventory;
        public int NumItemsInToolbar = 10;

        public Views.Region TabView;
        public ContainerView Inventory;
        public ContainerView Toolbar;

        public override void Initialize()
        {
            base.Initialize();

            if (EditorSceneManager.GetActiveScene().name == "TestMainUI")
            {
                TestUI();
            }
        }

        public void SetInventoryItem(ItemInstance itemInstance, int index)
        {
            var iiw = new ItemInstanceWrapper();
            iiw.ItemInstance = itemInstance;

            if (index < NumItemsInToolbar)
            {
                ToolbarItems[index] = iiw;
                Toolbar.SetChanged(() => Toolbar.Items); // ?
            }

            if (index < NumItemsInInventory)
            {
                InventoryItems[index] = iiw;
                Inventory.SetChanged(() => Inventory.Items); // ?
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TabView.Enabled = !TabView.Enabled;
                TabView.UpdateView();

                if (EditorSceneManager.GetActiveScene().name == "TestMainUI" && InventoryItems[0].ItemInstance.Item.Id != 10)
                {
                    SetInventoryItem(new ItemInstance(Locator.Get<ContentManager>().GetItem(10)), 0);
                }
            }
        }

        private void TestUI()
        {
            MainThreadQueue.Instantiate();
            Locator.Provide(new ContentManager("demo-world"));
            Locator.Provide(new TextureManager("demo-world"));

            var Items = new List<ItemInstance>();
            var cm = Locator.Get<ContentManager>();
            NumItemsInInventory = 50;
            for (int i = 0; i < NumItemsInInventory; i++)
            {
                Items.Add(new ItemInstance(cm.GetItem(i)));
            }

            InventoryItems = ItemInstanceWrapper.GetItemsWithBullshitWrapper(Items);
            ToolbarItems = InventoryItems.GetRange(0, NumItemsInToolbar);
        }
    }
}
