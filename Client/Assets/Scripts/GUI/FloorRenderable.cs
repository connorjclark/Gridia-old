﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class FloorRenderable : SpriteRenderable
    {
        public int FloorIndex { get; set; }

        public FloorRenderable(Vector2 pos, int floorIndex)
            : base(pos)
        {
            FloorIndex = floorIndex;
        }

        public override int GetSpriteIndex()
        {
            return FloorIndex;
        }

        public override Texture GetTexture(int spriteIndex)
        {
            var textures = Locator.Get<TextureManager>(); // :(
            return textures.Floors[spriteIndex / GridiaConstants.SPRITES_IN_SHEET];
        }
    }
}
