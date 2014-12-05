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
        private BulkViewer _bulkFloors;
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

            _bulkFloors = new BulkViewer(
                new Vector2(10 * GridiaConstants.SPRITE_SIZE, 0),
                () =>
                {
                    return _contentManager.FloorCount;
                },
                (int index) =>
                {
                    return new FloorRenderable(Vector2.zero, index);
                },
                (int index) =>
                {
                    var location = Locator.Get<TileMapView>().Focus.Position;
                    Locator.Get<ConnectionToGridiaServerHandler>().AdminMakeFloor(location, index);
                }
            );

            AddChild(_bulkItems);
            AddChild(_bulkFloors);
        }

        private class BulkViewer : RenderableContainer
        {
            private ExtendibleGrid _bulk = new ExtendibleGrid(new Vector2(0, 30));
            private int _currentPage;
            private int _perPage = 50;
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

                var prevPage = new Button(Vector2.zero, "Previous");
                prevPage.OnClick = () => { ShowPage(_currentPage - 1); };

                var nextPage = new Button(new Vector2(120, 0), "Next");
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
                var maxPage = GetNumElements() / _perPage - 1;
                if (page == -1)
                {
                    page = maxPage;
                }
                else if (page == maxPage + 1)
                {
                    page = 0;
                }
                _currentPage = page;

                _bulk.RemoveAllChildren();
                var firstItemToShow = page * _perPage;
                var lastItemToShow = Math.Min(GetNumElements(), firstItemToShow + _perPage);

                for (int i = firstItemToShow; i < lastItemToShow; i++)
                {
                    int elementIndex = i;
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
