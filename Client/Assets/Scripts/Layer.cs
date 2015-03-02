using System;
using UnityEngine;

namespace Gridia
{
    public class Layer
    {
        public readonly Func<Tile, int> GetTileData;
        public readonly Func<int, Texture> GetTexture;
        public readonly Func<Tile, Vector2> GetOffset;
        public readonly Func<Tile, int> GetWidth;
        public readonly Func<Tile, int> GetHeight;
        public readonly GameObject Renderable;
        public readonly Vector2[] uv;
        public readonly Mesh Mesh;
        private readonly TileMapView _view;

        public Layer (
            string name,
            TileMapView view,
            Func<Tile, int> getTileData,
            Func<int, Texture> getTexture,
            Func<Tile, Vector2> getOffset = null,
            Func<Tile, int> getWidth = null,
            Func<Tile, int> getHeight = null)
        {
            _view = view;
            GetTileData = getTileData;
            GetTexture = getTexture;
            GetOffset = getOffset ?? (tile => Vector2.zero);
            GetWidth = getWidth ?? (tile => 1);
            GetHeight = getHeight ?? (tile => 1);
            Renderable = InitRenderable(name);
            Mesh = Renderable.GetComponent<MeshFilter>().mesh; //smell
            uv = InitUV();
        }

        public void ApplyUV()
        {
            Mesh.uv = uv;
        }

        private GameObject InitRenderable(string name)
        {
            var renderable = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
            renderable.GetComponent<MeshFilter>().mesh = new Mesh();
            return renderable;
        }

        private Vector2[] InitUV()
        {
            var uv = new Vector2[_view.NumTiles * 4];
            for (var i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2();
            }
            return uv;
        }

        public void Delete()
        {
            UnityEngine.Object.Destroy(Renderable);
        }
    }
}
