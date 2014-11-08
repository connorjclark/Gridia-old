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
        public List<ItemInstance> Inventory { get; set; }
        public int slotsAcross = 10;
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        private float _slotSize;

        public InventoryGUI(Vector2 position, float scale)
        {
            Position = position;
            Inventory = new List<ItemInstance>();
            _slotSize = scale * GridiaConstants.SPRITE_SIZE;
        }

        public void Render() {
            MouseUpSlot = MouseDownSlot = -1;

            float width = slotsAcross * _slotSize;
            float height = (int)Math.Ceiling((float)Inventory.Count / slotsAcross) * _slotSize;
            GUILayout.BeginArea(new Rect(Position.x, Position.y, width, height));

            String tooltip = null;
            Rect tooltipRect = new Rect(0, 0, 0, 0);
            Vector2 mouse = Event.current.mousePosition;
            for (int i = 0; i < Inventory.Count; i++)
			{
                float x = (i % slotsAcross) * _slotSize;
                float y = (i / slotsAcross) * _slotSize;
                var item = Inventory[i];
                var slotRect = new Rect(x, y, _slotSize, _slotSize);
                RenderSlot(slotRect, item);
                bool slotContainsMouse = slotRect.Contains(mouse);
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
                        tooltip = item.Item.Name;
                        tooltipRect = new Rect(mouse.x, mouse.y - 60, 150, 30);
                    }
                }
			}

            GUILayout.EndArea();

            if (tooltip != null) {
                var globalRect = new Rect(Position.x + tooltipRect.x, Position.y + tooltipRect.y, tooltipRect.width, tooltipRect.height);
                GUI.Box(globalRect, tooltip);
            }
        }

        public void RenderSlot(Rect loc, ItemInstance item) {
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
