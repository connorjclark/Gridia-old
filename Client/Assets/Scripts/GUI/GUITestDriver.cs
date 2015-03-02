using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class GUITestDriver : MonoBehaviour
    {
        RenderableContainer _displayList;
        TabbedUI _tabWindow;

        public void Start() 
        {
            GridiaConstants.LoadGUISkins();

            ContentManager cm;
            Locator.Provide(cm = new ContentManager("demo-world"));
            Locator.Provide(new TextureManager("demo-world"));

            _tabWindow = new TabbedUI(new Vector2(float.MaxValue, 0)) {ScaleXY = 3};

            // inv window

            var invWindow = new ContainerWindow(Vector2.zero);
            _tabWindow.Add(123, invWindow, true);

            var inv = new List<ItemInstance>();
            for (var i = 0; i < 25; i++)
            {
                inv.Add(cm.GetItem(i).GetInstance(i));
            }

            invWindow.Set(inv, 0);

            invWindow.ScaleXY = 2f;

            // test window

            _displayList = new RenderableContainer(Vector2.zero);
            var container = new RenderableContainer(Vector2.zero);
            var container2 = new ExtendibleGrid(new Vector2(32, 64));
            container2.SetTilesAcross(6);

            _displayList.AddChild(container);
            _displayList.AddChild(container2);

            for (var x = 0; x < 5; x++)
            {
                var renderable = new ItemRenderable(new Vector2(x * 32, 0), cm.GetItem(x).GetInstance(1));
                container.AddChild(renderable);
                renderable.ScaleXY = Mathf.Sqrt(2);
            }
            container.ScaleXY = Mathf.Sqrt(2);

            for (var x = 0; x < 25; x++)
            {
                var renderable = new ItemRenderable(new Vector2(x * 32, 0), cm.GetItem(x).GetInstance(1));
                container2.AddChild(renderable);
            }
            container2.ScaleXY = 2;

            var testWindow = new GridiaWindow(Vector2.zero, "test");
            testWindow.AddChild(_displayList);
            _tabWindow.Add(11, testWindow, true);

            // components

            var components = new RenderableContainer(Vector2.zero);

            components.AddChild(new Button(Vector2.zero, "This is a test"));

            var componentsWindow = new GridiaWindow(Vector2.zero, "components");
            componentsWindow.AddChild(components);
            _tabWindow.Add(12, componentsWindow, true);
        }

        public void OnGUI() 
        {
            _tabWindow.Render();
            ToolTipRenderable.Instance.Render();
        }
    }
}
