using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ItemRenderable : Renderable
    {
        public ItemInstance Item { get; set; }

        public ItemRenderable(Rect rect, ItemInstance item)
            : base(rect)
        {
            Item = item;
        }

        public override void Render()
        {
            GUI.Box(Rect, "");

            if (Item.Item.Animations == null) return; // :(

            var textures = Locator.Get<TextureManager>(); // :(

            int spriteId = Item.Item.Animations[0];
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(Rect, textures.GetItemsTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }
    }
}
