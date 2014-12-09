using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public abstract class SpriteRenderable : Renderable
    {
        protected bool RenderBox { get; set; }

        public SpriteRenderable(Vector2 pos)
            : base(pos)
        {
            RenderBox = true;
            _rect.width = _rect.height = GridiaConstants.SPRITE_SIZE;
        }

        public abstract int GetSpriteIndex();

        public abstract Texture GetTexture(int spriteIndex);

        public override void Render()
        {
            base.Render();

            if (RenderBox) 
            {
                GUI.Box(Rect, "");
            }

            var textures = Locator.Get<TextureManager>(); // :(
            var spriteIndex = GetSpriteIndex();
            var texture = GetTexture(spriteIndex);

            int textureX = (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(Rect, texture, texCoords);
        }
    }
}
