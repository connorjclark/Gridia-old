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
        public int TileSelectedX
        {
            get { return TileSelected % TilesAcross; }
            set
            {
                var newX = Mathf.Clamp(value, 0, TilesAcross - 1);
                TileSelected = Mathf.Clamp(newX + TileSelectedY * TilesAcross, 0, NumChildren - 1);
            }
        }
        public int TileSelectedY
        {
            get { return TileSelected / TilesAcross; }
            set
            {
                var newY = Mathf.Clamp(value, 0, TilesColumn - 1);
                TileSelected = Mathf.Clamp(TileSelectedX + newY * TilesAcross, 0, NumChildren - 1);
            }
        }

        public ExtendibleGrid(Vector2 pos)
            : base(pos)
        {
            TilesAcross = 10;
        }

        public override void Render() 
        {
            base.Render();
            // :(
            if (TileSelected != -1 && TileSelected < NumChildren)
            {
                var rect = _children[TileSelected].Rect;
                GUI.Box(rect, "");
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
