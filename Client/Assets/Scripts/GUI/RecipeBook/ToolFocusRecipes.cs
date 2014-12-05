using Gridia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ToolFocusRecipes : ExtendibleGrid
    {
        private List<ItemUse> _uses;
        private int _currentSelection;
        private List<ItemRenderable> _products;

        public ToolFocusRecipes(Vector2 pos, List<ItemUse> uses)
            : base(pos)
        {
            _uses = uses;

            var focusItem = Locator.Get<ContentManager>().GetItem(uses[0].focus).GetInstance(1);
            var focus = new ItemRenderable(Vector2.zero, focusItem);

            AddChild(new Label(Vector2.zero, "+", true));
            AddChild(focus);
            AddChild(new Label(Vector2.zero, "=", true));

            if (_uses.Count > 1)
            {
                var nextButton = new Button(new Vector2(32, 32), ">");
                nextButton.OnClick = NextSelection;

                var prevButton = new Button(new Vector2(32, 32), "<");
                prevButton.OnClick = PreviousSelection;

                AddChild(prevButton);
                AddChild(nextButton);
            }

            DisplayCurrentSelection();
        }

        private void NextSelection()
        {
            _currentSelection = (_currentSelection + 1) % _uses.Count;
            DisplayCurrentSelection();
        }

        private void PreviousSelection()
        {
            if (_currentSelection == 0)
                _currentSelection = _uses.Count - 1;
            else
                _currentSelection--;

            DisplayCurrentSelection();
        }

        private void DisplayCurrentSelection()
        {
            if (_products != null)
                _products.ToList().ForEach(p => RemoveChild(p));

            var cm = Locator.Get<ContentManager>();
            var use = _uses[_currentSelection];
            _products = use.products
                .Select(product => cm.GetItem(product).GetInstance(1))
                .Select(product => new ItemRenderable(Vector2.zero, product))
                .ToList();

            _products.ForEach(product => AddChild(product));
        }
    }
}
