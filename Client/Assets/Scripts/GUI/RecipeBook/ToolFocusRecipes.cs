using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class ToolFocusRecipes : RenderableContainer
    {
        private readonly List<ItemUse> _uses;
        private int _currentSelection;
        private List<ItemRenderable> _products;
        private readonly Label _equalsLabel;
        private readonly Button _nextButton;

        public ToolFocusRecipes(Vector2 pos, List<ItemUse> uses)
            : base(pos)
        {
            _uses = uses;

            var focusItem = Locator.Get<ContentManager>().GetItem(uses[0].Focus).GetInstance();

            AddChild(new Label(new Vector2(5, 12), "+", true));
            AddChild(new ItemRenderable(new Vector2(15, 0), focusItem));
            AddChild(_equalsLabel = new Label(new Vector2(LastChildRight() + 5, 12), "=", true));

            if (_uses.Count > 1)
            {
                var prevButton = new Button(new Vector2(LastChildRight()+ 5, 0), "<") { OnClick = PreviousSelection };
                AddChild(prevButton);
                _nextButton = new Button(new Vector2(LastChildRight() + 40, 0), ">") { OnClick = NextSelection };
                AddChild(_nextButton);
            }

            DisplayCurrentSelection();
            CalculateRect();
        }

        private float LastChildRight()
        {
            var child = Children[Children.Count - 1];
            return child.X + child.Width;
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
            {
                _products.ForEach(RemoveChild);
                _products.Clear();
            }
            else
            {
                _products = new List<ItemRenderable>();
            }

            var cm = Locator.Get<ContentManager>();
            var use = _uses[_currentSelection];
            use.Products.ForEach(product =>
            {
                var item = cm.GetItem(product).GetInstance();
                float x;
                if (_products.Count == 0)
                {
                    x = _nextButton != null
                        ? (_nextButton.Width + _nextButton.X + 60)
                        : (_equalsLabel.Width + _equalsLabel.X + 10);
                }
                else
                {
                    x = LastChildRight() + 5;
                }
                var renderable = new ItemRenderable(Vector2.zero, item);
                AddChild(renderable);
                renderable.X = x;
                _products.Add(renderable);
            });
        }
    }
}
