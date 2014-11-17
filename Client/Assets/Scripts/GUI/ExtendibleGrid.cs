using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ExtendibleGrid : RenderableContainer
    {
        public int TilesAcross { get; private set; }
        public int TileSelected { get; set; }
        public int MouseDownTile { get; private set; }
        public int MouseUpTile { get; private set; }
        public int MouseOverTile { get; private set; }
        public float Width { get { if (Dirty) { CalculateRect(); } return base.Width; } }
        public float Height { get { if (Dirty) { CalculateRect(); } return base.Height; } }

        public ExtendibleGrid(Rect rect)
            : base(rect)
        {
            TilesAcross = 10;
            TileSelected = -1;
        }

        public override void Render() 
        {
            if (Event.current.type == EventType.Layout)
            {
                MouseOverTile = MouseDownTile = MouseUpTile = -1;
            }

            base.Render();

            for (int i = 0; i < NumChildren; i++)
            {
                var rect = _children[i].Rect;

                if (TileSelected == i)
                {
                    // :(
                    GUI.Box(rect, "");
                }
                bool slotContainsMouse = rect.Contains(Event.current.mousePosition);
                if (slotContainsMouse)
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        MouseDownTile = i;
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        MouseUpTile = i;
                    }
                    else
                    {
                        MouseOverTile = i;
                    }
                }
            }
        }

        public override void AddChild(Renderable child) 
        {
            base.AddChild(child);
            var i = NumChildren - 1;
            child.X = (i % TilesAcross) * GetTileWidth();
            child.Y = (i / TilesAcross) * GetTileHeight();
        }

        public void FitToWidth(float width)
        {
            width = Math.Max(width, GetTileWidth());
            TilesAcross = (int)(width / GetTileWidth());
            PositionTiles();
        }

        public void SetTilesAcross(int tilesAcross) 
        {
            TilesAcross = tilesAcross;
            PositionTiles();
        }

        // assumption: all tiles are the same size
        public float GetTileWidth() 
        {
            return NumChildren == 0 ? 0 : _children[0].Width;
        }

        public float GetTileHeight()
        {
            return NumChildren == 0 ? 0 : _children[0].Height;
        }

        private void PositionTile(Renderable tile, int index) 
        {
            tile.X = (index % TilesAcross) * GetTileWidth();
            tile.Y = (index / TilesAcross) * GetTileHeight();
        }

        private void PositionTiles() 
        {
            Dirty = true;
            for (int i = 0; i < NumChildren; i++)
            {
                PositionTile(_children[i], i);
            }
        }
    }
}
