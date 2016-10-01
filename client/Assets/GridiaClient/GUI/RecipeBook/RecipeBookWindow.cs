using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridia
{
    public class RecipeBookWindow : GridiaWindow
    {
        private readonly RenderableContainer _toolFocusGrid;
        private readonly List<List<ItemUse>> _usesWithFocus;
        private const int NumToShow = 5;
        private int _currentSelection;

        public RecipeBookWindow(Vector2 pos, ItemInstance tool)
            : base(pos, "Recipe Book")
        {
            _usesWithFocus = Locator.Get<ContentManager>().GetUses(tool)
                .GroupBy(use => use.Focus)
                .Select(usesWithFocus => usesWithFocus.ToList())
                .ToList();

            _rect.width = 150;

            var toolRenderable = new ItemRenderable(Vector2.zero, tool);
            AddChild(toolRenderable);

            if (_usesWithFocus.Count > NumToShow)
            {
                var prevButton = new Button(new Vector2(32, 0), "<") {OnClick = PreviousSelection};
                var nextButton = new Button(new Vector2(100, 0), ">") {OnClick = NextSelection};
                AddChild(prevButton);
                AddChild(nextButton);
            }

            _toolFocusGrid = new RenderableContainer(new Vector2(0, 32));
            AddChild(_toolFocusGrid);
            
            DisplayCurrentSelection();
        }

        private void NextSelection()
        {
            _currentSelection = _currentSelection + NumToShow;
            if (_currentSelection >= _usesWithFocus.Count)
            {
                _currentSelection = 0;
            }
            DisplayCurrentSelection();
        }

        private void PreviousSelection()
        {
            _currentSelection -= NumToShow;
            if (_currentSelection == -NumToShow)
            {
                _currentSelection = _usesWithFocus.Count - NumToShow;
            }
            else if (_currentSelection < 0)
            {
                _currentSelection = 0;
            }
            DisplayCurrentSelection();
        }

        private void DisplayCurrentSelection()
        {
            _toolFocusGrid.RemoveAllChildren();

            var lastIndex = Math.Min(_currentSelection + NumToShow, _usesWithFocus.Count);
            var y = 0.0f;
            for (var i = _currentSelection; i < lastIndex; i++)
            {
                var uses = _usesWithFocus[i];
                var toolFocusRecipes = new ToolFocusRecipes(new Vector2(0, y), uses);
                y += toolFocusRecipes.Height;
                _toolFocusGrid.AddChild(toolFocusRecipes);
            }

            WindowName = String.Format("Recipe Book: {0} - {1} out of {2}", _currentSelection + 1, lastIndex, _usesWithFocus.Count);
        }
    }
}
