using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridia
{
    public class TileMapView
    {
        public Vector2 Position { get; set; }
        public bool IsLighting { get; set; }

        private readonly TileMap _tileMap;
        private readonly TextureManager _textureManager;
        private readonly List<Layer> layers;
        private readonly Lighting lighting;

        private Shader shader;
        private Vector3[] vertices;
        private int width, height;

        public TileMapView(TileMap tileMap, TextureManager textureManager, float scale = 1.0f)
        {
            _tileMap = tileMap;
            _textureManager = textureManager;
            shader = FindShader();
            Scale = scale;
            layers = InitLayers();
            lighting = new Lighting(this);
            IsLighting = false;
        }

        public int NumTiles
        {
            get
            {
                return (width + 2) * (height + 2);
            }
        }

        private float _scale;
        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                float tileSize = GridiaConstants.SPRITE_SIZE * _scale;
                width = Mathf.CeilToInt(Screen.width / tileSize);
                height = Mathf.CeilToInt(Screen.height / tileSize);
                InitVertices();
            }
        }

        public float TileSize { get { return GridiaConstants.SPRITE_SIZE * _scale; } }

        public void Render()
        {
            Action<int[], int, int> setTriangles = (triangles, offset, tileIndex) =>
            {
                int vertexOffset = tileIndex * 4;
                triangles[offset + 0] = vertexOffset;
                triangles[offset + 1] = vertexOffset + 1;
                triangles[offset + 2] = vertexOffset + 2;
                triangles[offset + 3] = vertexOffset;
                triangles[offset + 4] = vertexOffset + 2;
                triangles[offset + 5] = vertexOffset + 3;
            };

            Action<Vector2[], int, int, int> setTextureCoords = (uv, tileIndex, tileX, tileY) =>
            {
                int offset = tileIndex * 4;

                float x1 = tileX * GridiaConstants.SPRITE_UV;
                float y1 = 1.0f - (tileY + 1) * GridiaConstants.SPRITE_UV; //smell
                float x2 = x1 + GridiaConstants.SPRITE_UV;
                float y2 = y1 + GridiaConstants.SPRITE_UV;

                uv[offset + 0].Set(x1, y1);
                uv[offset + 1].Set(x1, y2);
                uv[offset + 2].Set(x2, y2);
                uv[offset + 3].Set(x2, y1);
            };

            Action<Layer> drawLayer = layer =>
            {
                List<int[]> triangleBatches = new List<int[]>();
                List<Texture> textureBatches = new List<Texture>();
                List<Dictionary<string, object>> elementList = new List<Dictionary<string, object>>();

                int tileIndex = 0;
                int positionX = (int)Position.x;
                int positionY = (int)Position.y;

                ForEachInView((x, y) =>
                {
                    Tile tile = _tileMap.GetTile(x + positionX, y + positionY);
                    int dataIndex = layer.GetTileData(tile);
                    if (dataIndex != -1)
                    {
                        Texture textureForThisData = layer.GetTexture(dataIndex);
                        int tileX = (dataIndex % 100) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
                        int tileY = (dataIndex % 100) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;

                        setTextureCoords(layer.uv, tileIndex, tileX, tileY);

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic["tileIndex"] = tileIndex;
                        dic["texture"] = textureForThisData;
                        elementList.Add(dic);
                    }
                    tileIndex++;
                });

                if (elementList.Count == 0)
                {
                    return;
                }

                elementList.Sort((e1, e2) =>
                {
                    int tileSheet1Hash = e1["texture"].GetHashCode();
                    int tileSheet2Hash = e2["texture"].GetHashCode();
                    return tileSheet1Hash == tileSheet2Hash ? 0 : (tileSheet1Hash > tileSheet2Hash ? 1 : -1);
                });

                Action<Texture, int, int> drawBatch = (texture, count, offset) =>
                {
                    int[] tri = new int[count * 6];
                    for (int i = 0; i < count; i++)
                    {
                        setTriangles(tri, i * 6, (int)elementList[i + offset]["tileIndex"]);
                    }

                    triangleBatches.Add(tri);
                    textureBatches.Add(texture);
                };

                int tileOffset = 0;
                for (int i = 1; i < elementList.Count; i++)
                {
                    var currentElement = elementList[i];
                    var prevElement = elementList[i - 1];
                    Texture currentTexture = currentElement["texture"] as Texture;
                    Texture prevTexture = prevElement["texture"] as Texture;
                    if (currentTexture != prevTexture)
                    {
                        int count = (i - tileOffset) * 6;
                        int offset = tileOffset * 6 * 2;

                        count = i - tileOffset;
                        offset = tileOffset;

                        drawBatch(prevTexture, count, offset);
                        tileOffset = i;
                    }
                }

                drawBatch((Texture)elementList[elementList.Count - 1]["texture"], (elementList.Count - tileOffset), tileOffset);

                layer.mesh.vertices = vertices;
                layer.ApplyUV();

                Material[] materials = new Material[textureBatches.Count];

                for (int i = 0; i < triangleBatches.Count; i++)
                {
                    Material material = new Material(shader);
                    material.mainTexture = textureBatches[i];
                    materials[i] = material;
                }

                layer.renderable.renderer.materials = materials;

                layer.mesh.subMeshCount = triangleBatches.Count;
                for (int i = 0; i < triangleBatches.Count; i++)
                {
                    layer.mesh.SetTriangles(triangleBatches[i], i);
                }

                int shiftX = (int)(-Position.x % 1 * TileSize);
                int shiftY = (int)(-Position.y % 1 * TileSize);
                layer.renderable.transform.position = new Vector3(shiftX, shiftY, 0);
            };

            layers.ForEach(drawLayer);

            if (IsLighting)
            {
                UpdateLighting();
            }
        }

        private Shader FindShader() {
            var name = IsLighting ? "shaders/Sprites-Diffuse" : "shaders/Unlit-Alpha";
            return Resources.Load(name) as Shader;
        }

        private void UpdateLighting()
        {
            List<Vector3> lights = new List<Vector3>();
            int posX = (int)Position.x;
            int posY = (int)Position.y;
            ForEachInView((x, y) =>
            {
                var tile = _tileMap.GetTile(x + posX, y + posY);
                var lightIntensity = tile.Item.Item.Light;
                if (lightIntensity != 0)
                {
                    lights.Add(new Vector3(x, y, lightIntensity));
                }
            });
            lighting.SetLights(lights);
        }

        private void ForEachInView(Action<int, int> task)
        {
            for (int y = -1; y < height + 1; y++)
            {
                for (int x = -1; x < width + 1; x++)
                {
                    task(x, y);
                }
            }
        }

        private List<Layer> InitLayers()
        {
            var result = new List<Layer>();

            result.Add(new Layer(
                "Creature layer",
                this,
                tile => tile.Creature,
                data => _textureManager.GetCreaturesTexture(data / 100)
            ));

            result.Add(new Layer("Item layer", this, tile =>
            {
                int[] animations = tile.Item.Item.Animations;
                if (animations == null || animations.Length == 0)
                {
                    return -1;
                }
                int frame = (int)((Time.time * 6) % animations.Length);//smell
                return animations[frame];
            },
                data => _textureManager.GetItemsTexture(data / 100)
            ));

            result.Add(new Layer(
                "Floor layer",
                this,
                tile => tile.Floor,
                data => _textureManager.GetFloorsTexture(data / 100)
            ));
            
            return result;
        }

        private void InitVertices()
        {
            vertices = new Vector3[NumTiles * 4];
            int offset = 0;
            ForEachInView((x, y) =>
            {
                float tileSize = TileSize; //Does this save perfomance? Or would the compilier be smart enough?

                float x1 = x * tileSize;
                float y1 = y * tileSize;
                float x2 = x1 + tileSize;
                float y2 = y1 + tileSize;

                vertices[offset + 0] = new Vector3(x1, y1, 0);
                vertices[offset + 1] = new Vector3(x1, y2, 0);
                vertices[offset + 2] = new Vector3(x2, y2, 0);
                vertices[offset + 3] = new Vector3(x2, y1, 0);

                offset += 4;
            });
        }
    }
}