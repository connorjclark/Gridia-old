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
        private ItemRenderable _tool;
        private Vector2 _scrollPosition;

        public RecipeBookWindow(Rect rect, ItemInstance tool) 
            : base(rect, "Recipe Book")
        {
            var scale = 1.5f;

            _toolFocusGrid = new ExtendibleGrid(new Rect(0, scale * 32, 0, 0));

            var usesWithTool = Locator.Get<ContentManager>().GetUses(tool);
            usesWithTool
                .GroupBy(use => use.focus)
                .ToList()
                .ForEach(usesWithFocus => {
                    var uses = usesWithFocus.ToList();
                    _toolFocusGrid.AddChild(new ToolFocusRecipes(new Rect(0, 0, 1000, 32 * scale), uses, scale));
                });
            _toolFocusGrid.SetTilesAcross(1);
            _tool = new ItemRenderable(new Rect(0, 0, 32 * scale, 32 * scale), tool);
        }

        protected override void RenderContents()
        {
            _tool.Render();

            var middleX = (Width - BorderSize * 2 - _toolFocusGrid.Width) / 2;

            var pos = new Rect(middleX, _toolFocusGrid.Y, _toolFocusGrid.Width + 25, Height - BorderSize * 2 - _tool.Height);
            var view = new Rect(0, 0, _toolFocusGrid.Width, _toolFocusGrid.Height);

            _scrollPosition = GUI.BeginScrollView(pos, _scrollPosition, view);
            _toolFocusGrid.Render();
            GUI.EndScrollView();
        }

        protected override void Resize()
        {
            base.Resize();
            Width = Math.Max(Width, _toolFocusGrid.Width + 50);
            _tool.X = (Width - _tool.Width) / 2;
        }
    }
}
