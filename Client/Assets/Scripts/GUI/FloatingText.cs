﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class FloatingText : Label
    {
        public Vector3 Coord { get; set; }
        public int Life { get; set; }

        public FloatingText(Vector3 coord, String text)
            : base(Vector2.zero, text)
        {
            Coord = coord;
            Life = 400;
        }

        public void Reposition(float tileSize, Vector3 playerLocation) 
        {
            var relative = Coord - playerLocation;
            X = (relative.x - 1) * tileSize;
            Y = Screen.height - (int)((relative.y + 1.5) * tileSize + 100 - Life / 8);
        }

        public void Tick() 
        {
            Life--;
        }

        public override void Render()
        {
            GUI.Box(Rect, "");
            base.Render();
        }
    }
}
