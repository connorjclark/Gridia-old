using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryGUI : GridiaWindow
    {
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
                _windowRect = new Rect(_windowRect.x, _windowRect.y, width + BorderSize * 2, height + BorderSize * 2);
            }
        }
        public int slotsAcross = 10;
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        private float _slotSize;
        private float _scale;
        private String _tooltip = null;
        private Rect _tooltipRect = new Rect(0, 0, 0, 0);

        public InventoryGUI(Vector2 position, float scale) : base(position, "Inventory")
        {
            ResizeOnVertical = false;
            Inventory = new List<ItemInstance>();
            _slotSize = scale * GridiaConstants.SPRITE_SIZE;
            _scale = scale;
        }

        protected override void RenderContents()
        {
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
        }

        public void Render() {
            if (Event.current.type == EventType.Layout)
            {
                MouseUpSlot = MouseDownSlot = -1;
                _tooltip = null;
                _tooltipRect = new Rect(0, 0, 0, 0);
            }
            base.Render();
            RenderTooltip();
        }

        protected override void Resize()
        {
            base.Resize();
            _windowRect.width = Math.Max(_windowRect.width, BorderSize * 2 + _slotSize);
            slotsAcross = (int)(_windowRect.width / _slotSize);
            int height = (int)(Math.Ceiling((float)Inventory.Count / slotsAcross) * _slotSize + BorderSize * 2);
            _windowRect.height = height;
        }

        private void RenderTooltip() 
        {
            if (_tooltip != null)
            {
                var globalRect = new Rect(Event.current.mousePosition.x + _tooltipRect.x, Event.current.mousePosition.y + _tooltipRect.y, _tooltipRect.width, _tooltipRect.height);
                GUI.Box(globalRect, _tooltip);
            }
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
