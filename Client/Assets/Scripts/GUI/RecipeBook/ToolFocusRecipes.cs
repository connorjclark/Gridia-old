using Gridia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ToolFocusRecipes : RenderableContainer
    {
        private List<ItemUse> _uses;
        private int _currentSelection;
        private List<ItemRenderable> _products;
        private float _textWidth = 20;
        private float _itemSize = 32;

        public ToolFocusRecipes(Rect rect, List<ItemUse> uses, float scale)
            : base(rect)
        {
            _uses = uses;

            _textWidth *= scale;
            _itemSize *= scale;

            var focusItem = Locator.Get<ContentManager>().GetItem(uses[0].focus).GetInstance(1);
            var focus = new ItemRenderable(new Rect(_textWidth, 0, _itemSize, _itemSize), focusItem);
            AddChild(focus);

            if (_uses.Count > 1)
            {
                var next = new Button(new Rect(_itemSize * 1.5f + _textWidth * 2, 0, _itemSize, _itemSize / 2), ">");
                var prev = new Button(new Rect(_itemSize * 1.5f + _textWidth * 2, _itemSize / 2, _itemSize, _itemSize / 2), "<");
                AddChild(next);// :( AddChildren?
                AddChild(prev);
                next.OnClick = NextSelection;
                prev.OnClick = PreviousSelection;
            }

            DisplayCurrentSelection();
        }

        public override void Render()
        {
            base.Render();
            GUI.BeginGroup(Rect);
            GUI.Label(new Rect(0, _itemSize / 2, _textWidth, _itemSize), "+");
            GUI.Label(new Rect(_textWidth + _itemSize * 1.5f, _itemSize / 2, _textWidth, _itemSize), "=");
            GUI.EndGroup();
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
                _products.ForEach(p => RemoveChild(p));

            var cm = Locator.Get<ContentManager>();
            var use = _uses[_currentSelection];
            var i = 3;
            _products = use.products
                .Select(product => cm.GetItem(product).GetInstance(1))
                .Select(product => new ItemRenderable(new Rect(i++ * _itemSize + _textWidth, 0, _itemSize, _itemSize), product))
                .ToList();

            _products.ForEach(product => AddChild(product));
        }
    }
}
