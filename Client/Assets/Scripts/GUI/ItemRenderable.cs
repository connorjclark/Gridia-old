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

        public ItemRenderable(Vector2 pos, ItemInstance item)
            : base(pos)
        {
            _rect.width = _rect.height = GridiaConstants.SPRITE_SIZE;
            Item = item;
            ToolTip = () => Item.Item.Id != 0 ? Item.ToString() : null;
        }

        public override void Render()
        {
            base.Render();

            GUI.Box(Rect, "");

            var animations = Item.Item.Animations;
            if (animations == null) return; // :(

            int frame = (int)((Time.time * 6) % animations.Length); //smell :(

            var textures = Locator.Get<TextureManager>(); // :(

            int spriteId = animations[frame];
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(Rect, textures.GetItemsTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }
    }
}
