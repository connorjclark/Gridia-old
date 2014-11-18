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
        public int TilesColumn { get { return Mathf.CeilToInt((float)NumChildren / TilesAcross); } }
        public int TileSelected { get; set; }

        public ExtendibleGrid(Vector2 pos)
            : base(pos)
        {
            TilesAcross = 10;
            TileSelected = -1;
        }

        public override void Render() 
        {
            base.Render();

            for (int i = 0; i < NumChildren; i++)
            {
                var rect = _children[i].Rect;

                if (TileSelected == i)
                {
                    // :(
                    GUI.Box(rect, "");
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
            TilesAcross = Math.Max(1, tilesAcross);
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
            var x = (index % TilesAcross) * GetTileWidth();
            var y = (index / TilesAcross) * GetTileHeight();
            tile.Rect = new Rect(x, y, tile.Width, tile.Height);
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
