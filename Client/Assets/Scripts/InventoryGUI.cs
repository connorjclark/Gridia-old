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

        public InventoryGUI(Vector2 position)
        {
            Position = position;
            Inventory = new List<ItemInstance>();
        }

        public void Render() {
            MouseUpSlot = MouseDownSlot = -1;

            int width = slotsAcross * 32;
            int height = (int)Math.Ceiling((float)Inventory.Count / slotsAcross) * 32;
            GUILayout.BeginArea(new Rect(Position.x, Position.y, width, height));

            String tooltip = null;
            Rect tooltipRect = new Rect(0, 0, 0, 0);
            Vector2 mouse = Event.current.mousePosition;
            for (int i = 0; i < Inventory.Count; i++)
			{
                int x = (i % slotsAcross) * 32;
                int y = (i / slotsAcross) * 32;
                var item = Inventory[i];
                var slotRect = new Rect(x, y, 32, 32);
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
                        tooltipRect = new Rect(mouse.x, mouse.y - 30, 150, 30);
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
