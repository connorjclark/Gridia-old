using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryGUI
    {
        public List<ItemInstance> Inventory { private get; set; }
        public int slotsAcross = 10;
        public int Left { get; set; }
        public int Top { get; set; }

        public InventoryGUI(int left, int top)
        {
            Left = left;
            Top = top;
            Inventory = new List<ItemInstance>();
        }

        public int getSlotIndexUnderPoint(Vector2 point) {
            int x = (int)(point.x / 32);
            int y = (int)((Screen.height - point.y) / 32);

            x = Math.Max(0, Math.Min(Screen.width, x));
            y = Math.Max(0, Math.Min(Screen.height, y));

            int slotsColumn = (int)Math.Ceiling(Inventory.Count / (float)slotsAcross);

            int slotIndex = x + (slotsColumn-y - 1) * slotsAcross;

            if (x >= slotsAcross || y >= slotsColumn || slotIndex >= Inventory.Count)
            {
                return -1;
            }

            return slotIndex;
        }

        public String tooltip;
        public Rect toolRect;

        public void Render() {
            tooltip = null;
            Vector2 mouse = Event.current.mousePosition;
            for (int i = 0; i < Inventory.Count; i++)
			{
                int x = i % slotsAcross;
                int y = i / slotsAcross;
                var item = Inventory[i];
                RenderSlot(x * 32 + Left, y * 32 + Top, item);
                Rect loc = new Rect(x * 32 + Left, y * 32 + Top, 32, 32); // :(
                if (loc.Contains(mouse))
                {
                    tooltip = item.Item.Name;
                    toolRect = new Rect(mouse.x, mouse.y - 30, 150, 30);
                }
			}
        }

        public void RenderSlot(int x, int y, ItemInstance item) {
            var textures = Locator.Get<TextureManager>();

            Rect loc = new Rect(x, y, 32, 32);
            GUI.Box(loc, "");

            if (item.Item.Animations == null) return; // :(
            int spriteId = item.Item.Animations[0];
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            Rect texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(loc, textures.GetItemsTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }
    }
}
