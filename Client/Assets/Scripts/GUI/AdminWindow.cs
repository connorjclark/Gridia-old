using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class AdminWindow : GridiaWindow
    {
        private BulkViewer _bulkItems;
        private ContentManager _contentManager;

        public AdminWindow(Vector2 pos)
            : base(pos, "Admin Control Panel")
        {
            _contentManager = Locator.Get<ContentManager>();

            _bulkItems = new BulkViewer(
                Vector2.zero,
                () =>
                {
                    return _contentManager.ItemCount;
                },
                (int index) =>
                {
                    return new ItemRenderable(Vector2.zero, _contentManager.GetItem(index).GetInstance());
                },
                (int index) =>
                {
                    var location = Locator.Get<TileMapView>().Focus.Position;
                    Locator.Get<ConnectionToGridiaServerHandler>().AdminMakeItem(location, index);
                }
            );

            AddChild(_bulkItems);
        }

        private class BulkViewer : RenderableContainer
        {
            private ExtendibleGrid _bulk = new ExtendibleGrid(new Vector2(0, 30));
            private int _currentPage;
            private int _perPage = 100;
            private Vector2 vector2;

            public BulkViewer(
                Vector2 pos,
                Func<int> getNumElements,
                Func<int, Renderable> getElement,
                Action<int> onElementClick
            )
                : base(pos)
            {
                _bulk.TileSelected = -1;

                GetNumElements = getNumElements;
                GetElement = getElement;
                OnElementClick = onElementClick;

                var prevPage = new Button(Vector2.zero, 100, 20, "Previous");
                prevPage.OnClick = () => { ShowPage(_currentPage - 1); };

                var nextPage = new Button(new Vector2(120, 0), 100, 20, "Next");
                nextPage.OnClick = () => { ShowPage(_currentPage + 1); };

                ShowPage(0);

                AddChild(prevPage);
                AddChild(nextPage);
                AddChild(_bulk);
            }

            private Func<int> GetNumElements;

            private Func<int, Renderable> GetElement;

            private Action<int> OnElementClick;

            private void ShowPage(int page)
            {
                page = Mathf.Clamp(page, 0, GetNumElements() / _perPage);
                _currentPage = page;

                _bulk.RemoveAllChildren();
                var firstItemToShow = page * _perPage;
                var lastItemToShow = Math.Min(GetNumElements(), firstItemToShow + _perPage);

                for (int i = firstItemToShow; i < lastItemToShow; i++)
                {
                    int elementIndex = i;
                    //var item = new ItemRenderable(Vector2.zero, _contentManager.GetItem(itemIndex).GetInstance());
                    var element = GetElement(i);
                    element.OnClick = () =>
                    {
                        OnElementClick(elementIndex);
                    };
                    _bulk.AddChild(element);
                }
            }
        }
    }
}
