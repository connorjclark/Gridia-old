using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class GUITestDriver : MonoBehaviour
    {
        RenderableContainer displayList;
        TabbedUI tabWindow;

        public void Start() 
        {
            ContentManager cm;
            Locator.Provide(cm = new ContentManager("TestWorld"));
            Locator.Provide(new TextureManager("TestWorld"));

            tabWindow = new TabbedUI(new Vector2(float.MaxValue, 0));
            tabWindow.ScaleXY = 3;

            // inv window

            var invWindow = new InventoryWindow(Vector2.zero);
            tabWindow.Add(123, invWindow);

            var inv = new List<ItemInstance>();
            for (int i = 0; i < 25; i++)
            {
                inv.Add(cm.GetItem(i).GetInstance(i));
            }

            invWindow.Items = inv;

            invWindow.ScaleXY = 2f;

            // test window

            displayList = new RenderableContainer(Vector2.zero);
            var container = new RenderableContainer(Vector2.zero);
            var container2 = new ExtendibleGrid(new Vector2(32, 64));
            container2.SetTilesAcross(6);

            displayList.AddChild(container);
            displayList.AddChild(container2);

            for (int x = 0; x < 5; x++)
            {
                var renderable = new ItemRenderable(new Vector2(x * 32, 0), cm.GetItem(x).GetInstance(1));
                container.AddChild(renderable);
                renderable.ScaleXY = Mathf.Sqrt(2);
            }
            container.ScaleXY = Mathf.Sqrt(2);

            for (int x = 0; x < 25; x++)
            {
                var renderable = new ItemRenderable(new Vector2(x * 32, 0), cm.GetItem(x).GetInstance(1));
                container2.AddChild(renderable);
            }
            container2.ScaleXY = 2;

            var testWindow = new GridiaWindow(Vector2.zero, "test");
            testWindow.AddChild(displayList);
            tabWindow.Add(11, testWindow);
        }

        public void OnGUI() 
        {
            tabWindow.Render();
            ToolTipRenderable.instance.Render();
        }
    }
}
