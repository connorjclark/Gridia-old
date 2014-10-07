using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridia
{
    public class TileMapView
    {
        public static int OFF_GRID_TILES = 100;

        public Player Focus { get; set; }
        public bool IsLighting { get; set; }
        public int NumGridTiles { get { return (width + 2) * (height + 2); } }
        public int NumTiles { get { return NumGridTiles + OFF_GRID_TILES; } }
        public float TileSize { get { return GridiaConstants.SPRITE_SIZE * _scale; } }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                float tileSize = GridiaConstants.SPRITE_SIZE * _scale;
                width = Mathf.CeilToInt(Screen.width / tileSize);
                height = Mathf.CeilToInt(Screen.height / tileSize);
                InitGridVertices();
            }
        }

        private readonly TileMap _tileMap;
        private readonly TextureManager _textureManager;
        private readonly List<Layer> layers;
        private readonly Lighting lighting;
        private readonly Shader shader;
        private Vector3[] _gridVertices;
        private int width, height;
        private float _scale;

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
                int positionX = (int)Focus.Position.x;
                int positionY = (int)Focus.Position.y;
                int numTilesOffGrid = 0;

                ForEachInView((x, y) =>
                {
                    Tile tile = _tileMap.GetTile(x + positionX, y + positionY);
                    int dataIndex = layer.GetTileData(tile);
                    if (dataIndex != -1)
                    {
                        Texture textureForThisData = layer.GetTexture(dataIndex);
                        int tileX = (dataIndex % 100) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
                        int tileY = (dataIndex % 100) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic["texture"] = textureForThisData;

                        Vector2 tileOffset = layer.GetOffset(tile);
                        if (tileOffset != Vector2.zero && numTilesOffGrid < OFF_GRID_TILES)
                        {
                            int offTileIndex = NumGridTiles + numTilesOffGrid;
                            SetTileVertices(offTileIndex * 4, x + tileOffset.x, y + tileOffset.y);
                            setTextureCoords(layer.uv, offTileIndex, tileX, tileY);
                            dic["tileIndex"] = offTileIndex;
                            numTilesOffGrid++;
                        }
                        else
                        {
                            setTextureCoords(layer.uv, tileIndex, tileX, tileY);
                            dic["tileIndex"] = tileIndex;
                        }
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

                int offsetForNextBatch = 0;
                for (int i = 1; i < elementList.Count; i++)
                {
                    var currentElement = elementList[i];
                    var prevElement = elementList[i - 1];
                    Texture currentTexture = currentElement["texture"] as Texture;
                    Texture prevTexture = prevElement["texture"] as Texture;
                    if (currentTexture != prevTexture)
                    {
                        int count = (i - offsetForNextBatch) * 6;
                        int offset = offsetForNextBatch * 6 * 2;

                        count = i - offsetForNextBatch;
                        offset = offsetForNextBatch;

                        drawBatch(prevTexture, count, offset);
                        offsetForNextBatch = i;
                    }
                }

                drawBatch((Texture)elementList[elementList.Count - 1]["texture"], (elementList.Count - offsetForNextBatch), offsetForNextBatch);

                layer.mesh.vertices = _gridVertices;

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

                Vector2 renderablePosition = TileSize * Utilities.Vector2Residual(-Focus.Position);
                layer.renderable.transform.position = Utilities.Vector2Floor(renderablePosition);
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
            int posX = (int)Focus.Position.x;
            int posY = (int)Focus.Position.y;
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
                tile => {
                    if (tile.Creature == null) {
                        return -1;
                    }
                    return tile.Creature.Image;
                },
                data => _textureManager.GetCreaturesTexture(data / 100),
                tile => {
                    return tile.Creature.Offset;
                }
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
                data => _textureManager.GetItemsTexture(data / 100),
                tile =>
                {
                    if (tile.Item.Item.Name.Contains("Rose"))
                    {
                        return 0.1f * Utilities.Vector2FromAngle(Time.time + tile.GetHashCode());
                    }
                    else
                    {
                        return Vector2.zero;
                    }
                }
            ));

            result.Add(new Layer(
                "Floor layer",
                this,
                tile => tile.Floor,
                data => _textureManager.GetFloorsTexture(data / 100)
            ));
            
            return result;
        }

        private void InitGridVertices()
        {
            _gridVertices = new Vector3[NumTiles * 4];
            int offset = 0;
            ForEachInView((x, y) =>
            {
                SetTileVertices(offset, x, y);
                offset += 4;
            });
        }

        private void SetTileVertices(int offset, float x, float y) {
            float x1 = x * TileSize;
            float y1 = y * TileSize;
            float x2 = x1 + TileSize;
            float y2 = y1 + TileSize;
            _gridVertices[offset + 0] = new Vector3(x1, y1, 0);
            _gridVertices[offset + 1] = new Vector3(x1, y2, 0);
            _gridVertices[offset + 2] = new Vector3(x2, y2, 0);
            _gridVertices[offset + 3] = new Vector3(x2, y1, 0);
        }
    }
}