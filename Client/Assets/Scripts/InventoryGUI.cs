using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryGUI
    {
        public Vector2 Position { get; set; }
        private List<ItemInstance> _inventory;
        public List<ItemInstance> Inventory
        {
            get
            {
                return _inventory;
            }
            set
            {
                _inventory = value;
                float width = slotsAcross * _slotSize;
                float height = (int)Math.Ceiling((float)Inventory.Count / slotsAcross) * _slotSize;
                _windowRect = new Rect(Position.x, Position.y, width, height + _borderHeight * 2);
            }
        }
        public int slotsAcross = 10;
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        public bool MouseOver { get; private set; }
        public bool ResizingWindow { get; private set; }
        private float _slotSize;
        private float _scale;
        private Rect _windowRect;
        private String _tooltip = null;
        private Rect _tooltipRect = new Rect(0, 0, 0, 0);
        private int _borderHeight = 20;

        public InventoryGUI(Vector2 position, float scale)
        {
            Position = position;
            Inventory = new List<ItemInstance>();
            _slotSize = scale * GridiaConstants.SPRITE_SIZE;
            _scale = scale;
        }

        public void Render() {
            if (Event.current.type == EventType.Layout)
            {
                MouseUpSlot = MouseDownSlot = -1;
                MouseOver = false;
                _tooltip = null;
                _tooltipRect = new Rect(0, 0, 0, 0);
            }

            if (Event.current.type == EventType.MouseUp)
            {
                ResizingWindow = false;
            }

            if (ResizingWindow) 
            {
                Resize();
            }

            MouseOver = _windowRect.Contains(Event.current.mousePosition);

            _windowRect = GUI.Window(0, _windowRect, windowId =>
            {
                RenderDragAndResize();

                GUILayout.BeginArea(new Rect(5, _borderHeight, 1000, 1000));

                for (int i = 0; i < Inventory.Count; i++)
                {
                    float x = (i % slotsAcross) * _slotSize;
                    float y = (i / slotsAcross) * _slotSize;
                    var item = Inventory[i];
                    var slotRect = new Rect(x, y, _slotSize, _slotSize);
                    RenderSlot(slotRect, item);
                    bool slotContainsMouse = slotRect.Contains(Event.current.mousePosition);
                    if (slotContainsMouse)
                    {
                        if (Event.current.type == EventType.MouseDown)
                        {
                            MouseDownSlot = i;
                        }
                        else if (Event.current.type == EventType.MouseUp)
                        {
                            MouseUpSlot = i;
                        }
                        else
                        {
                            _tooltip = item.Item.Name;
                            _tooltipRect = new Rect(0, 0 - 60, 150, 30);
                        }
                    }
                }

                GUILayout.EndArea();
            }, "Inventory");

            ClampPosition();

            RenderTooltip();
        }

        private void Resize() {
            _windowRect.width = Event.current.mousePosition.x - _windowRect.x + 10;
            _windowRect.width = Mathf.Clamp(_windowRect.width, _slotSize, Screen.width);
            slotsAcross = (int)(_windowRect.width / _slotSize);
            int height = (int)(Math.Ceiling((float)Inventory.Count / slotsAcross) * _slotSize) + _borderHeight * 2;
            _windowRect.height = height;
        }

        private void RenderDragAndResize()
        {
            GUI.DragWindow(new Rect(0, 0, _windowRect.width - 40, 20));
            var resizeRect = new Rect(_windowRect.width - 40, 0, 40, 20);
            if (Event.current.type == EventType.mouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                ResizingWindow = true;
            }
            GUI.Label(resizeRect, " ◄►");
        }

        private void RenderTooltip() 
        {
            if (_tooltip != null)
            {
                var globalRect = new Rect(Event.current.mousePosition.x + _tooltipRect.x, Event.current.mousePosition.y + _tooltipRect.y, _tooltipRect.width, _tooltipRect.height);
                GUI.Box(globalRect, _tooltip);
            }
        }

        private void ClampPosition() 
        {
            var maxWidth = Math.Max(0, Screen.width - _windowRect.width);
            var maxHeight = Math.Max(0, Screen.height - _windowRect.height);
            _windowRect.x = Mathf.Clamp(_windowRect.x, 0, maxWidth);
            _windowRect.y = Mathf.Clamp(_windowRect.y, 0, maxHeight);
        }

        public void RenderSlot(Rect loc, ItemInstance item) 
        {
            GUI.Box(loc, "");

            if (item.Item.Animations == null) return; // :(

            var textures = Locator.Get<TextureManager>();

            int spriteId = item.Item.Animations[0];
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(loc, textures.GetItemsTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }
    }
}
