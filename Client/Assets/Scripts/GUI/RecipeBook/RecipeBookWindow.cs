using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class RecipeBookWindow : GridiaWindow
    {
        private ExtendibleGrid _toolFocusGrid;
        private ScrollView _scrollView;

        public RecipeBookWindow(Vector2 pos, ItemInstance tool) 
            : base(pos, "Recipe Book")
        {
            _rect.width = 150;
            _toolFocusGrid = new ExtendibleGrid(new Vector2(0, 32));

            var usesWithTool = Locator.Get<ContentManager>().GetUses(tool);
            usesWithTool
                .GroupBy(use => use.focus)
                .ToList()
                .ForEach(usesWithFocus => {
                    var uses = usesWithFocus.ToList();
                    _toolFocusGrid.AddChild(new ToolFocusRecipes(Vector2.zero, uses));
                });
            _toolFocusGrid.SetTilesAcross(1);

            AddChild(new ItemRenderable(new Vector2(0, 0), tool));
            _toolFocusGrid.Parent = this; // hack to get the scale... :(
            AddChild(_scrollView = new ScrollView(new Vector2(0, 32), Width, Height - 32, _toolFocusGrid));
        }

        public override void Render()
        {
            base.Render();
            _toolFocusGrid.SetTilesAcross(1); // :(
            _scrollView.Width = _toolFocusGrid.Width / _toolFocusGrid.TrueScale.x + BorderSize * 2;
        }

        protected override void Resize()
        {
            base.Resize();
        }
    }
}
