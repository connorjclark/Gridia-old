using System;
using UnityEngine;

namespace Gridia
{
    public class Layer
    {
        public readonly Func<Tile, int> GetTileData;
        public readonly Func<int, Texture> GetTexture;
        public readonly Func<Tile, Vector2> GetOffset;
        public readonly GameObject renderable;
        public readonly Vector2[] uv;
        public readonly Mesh mesh;
        private readonly TileMapView _view;

        public Layer (
            string name,
            TileMapView view,
            Func<Tile, int> getTileData,
            Func<int, Texture> getTexture,
            Func<Tile, Vector2> getOffset = null)
        {
            _view = view;
            GetTileData = getTileData;
            GetTexture = getTexture;
            if (getOffset == null)
            {
                GetOffset = tile => Vector2.zero;
            }
            else
            {
                GetOffset = getOffset;
            }
            renderable = InitRenderable(name);
            mesh = renderable.GetComponent<MeshFilter>().mesh; //smell
            uv = InitUV();
        }

        public void ApplyUV()
        {
            mesh.uv = uv;
        }

        private GameObject InitRenderable(string name)
        {
            GameObject renderable = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
            renderable.GetComponent<MeshFilter>().mesh = new Mesh();
            return renderable;
        }

        private Vector2[] InitUV()
        {
            Vector2[] uv = new Vector2[_view.NumTiles * 4];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2();
            }
            return uv;
        }
    }
}
