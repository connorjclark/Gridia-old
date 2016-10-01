namespace Gridia
{
    using System;

    using UnityEngine;

    public class ExtendibleGrid : RenderableContainer
    {
        #region Constructors

        public ExtendibleGrid(Vector2 pos)
            : base(pos)
        {
            TilesAcross = 10;
            ShowSelected = false;
            SelectedColor = new Color32(255, 255, 0, 50);
        }

        #endregion Constructors

        #region Properties

        public Color SelectedColor
        {
            get; set;
        }

        public bool ShowSelected
        {
            get; set;
        }

        public int TilesAcross
        {
            get; private set;
        }

        public int TilesColumn
        {
            get { return Mathf.CeilToInt((float)NumChildren / TilesAcross); }
        }

        public int TileSelected
        {
            get; set;
        }

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

        #endregion Properties

        #region Methods

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

        public float GetTileHeight()
        {
            return NumChildren == 0 ? 0 : Children[0].Height;
        }

        // assumption: all tiles are the same size
        public float GetTileWidth()
        {
            return NumChildren == 0 ? 0 : Children[0].Width;
        }

        public override void Render()
        {
            base.Render();
            // :(
            if (!ShowSelected || TileSelected == -1 || TileSelected >= NumChildren) return;
            var rect = Children[TileSelected].Rect;
            GridiaConstants.GUIDrawSelector(rect, SelectedColor); // :(
        }

        public void SetTilesAcross(int tilesAcross)
        {
            TilesAcross = Math.Max(1, tilesAcross);
            PositionTiles();
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
            for (var i = 0; i < NumChildren; i++)
            {
                PositionTile(Children[i], i);
            }
        }

        #endregion Methods
    }
}