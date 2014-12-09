using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridia
{
    public class TileMapView
    {
        public static int OFF_GRID_TILES = 400; // stress test this limit

        public Creature Focus { 
            get 
            {
                return _tileMap.GetCreature(FocusId);
            } 
        }
        public int FocusId { get; set; }
        public Vector3 FocusPosition
        { 
            get {
                var cre = Focus;
                var pos = cre == null ? Vector3.zero : cre.Position;
                return pos - new Vector3(width / 2, height / 2, 0);
            }
        }
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
                if (layers != null)
                {
                    layers.ForEach(layer => {
                        layer.Delete();
                    });
                }
                layers = InitLayers();
            }
        }

        private TileMap _tileMap;
        private TextureManager _textureManager;
        private List<Layer> layers;
        private Lighting lighting;
        private Shader shader;
        private Vector3[] _gridVertices;
        public int width, height;
        private float _scale;

        public TileMapView(TileMap tileMap, TextureManager textureManager, float scale = 1.0f)
        {
            _tileMap = tileMap;
            _textureManager = textureManager;
            shader = FindShader();
            Scale = scale;
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
                var pos = FocusPosition;
                int positionX = (int)pos.x;
                int positionY = (int)pos.y;
                int positionZ = (int)pos.z;
                int numTilesOffGrid = 0;

                ForEachInView((x, y) =>
                {
                    Tile tile = _tileMap.GetTile(x + positionX, y + positionY, positionZ);
                    int dataIndex = layer.GetTileData(tile);
                    if (dataIndex != -1)
                    {
                        Texture textureForThisData;

                        //hack for template :(
                        if (dataIndex <= -2)
                        {
                            var templateId = dataIndex + 3;
                            var typeToMatch = templateId == 0 ? 1 : 0;
                            textureForThisData = _textureManager.Templates[0];
                            dataIndex = UseTemplate(templateId, typeToMatch, x + positionX, y + positionY, positionZ);
                        }
                        else
                        {
                            textureForThisData = layer.GetTexture(dataIndex);
                        }

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

                var layerz = layer.renderable.transform.position.z;
                Vector3 renderablePosition = TileSize * Utilities.Vector3Residual(-pos);
                var newLayerPos = Utilities.Vector3Floor(renderablePosition);
                newLayerPos.z = layerz; // :(
                layer.renderable.transform.position = newLayerPos;
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
            int posZ = (int)Focus.Position.z;
            ForEachInView((x, y) =>
            {
                var tile = _tileMap.GetTile(x + posX, y + posY, posZ);
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
                "Floor layer",
                this,
                tile => {
                    if (tile.Floor == 0) {
                        return -2;
                    }
                    if (tile.Floor == 1) {
                        return -3;
                    }
                    return tile.Floor;
                },
                data => _textureManager.Floors[data / 100]
            ));

            result.Add(new Layer(
                "Item layer", 
                this, 
                tile =>
                {
                    int[] animations = tile.Item.Item.Animations;
                    if (animations == null || animations.Length == 0)
                    {
                        return -1;
                    }
                    int frame = (int)((Time.time * 6) % animations.Length);//smell :(
                    return animations[frame];
                },
                data => _textureManager.Items[data / 100],
                tile =>
                {
                    if (tile.Item.Item.Name.Contains("Cut"))
                    {
                        return 0.1f * Utilities.Vector2FromAngle(Time.time + tile.GetHashCode());
                    }
                    else
                    {
                        return Vector2.zero;
                    }
                }
            ));

            for (int i = 0; i < result.Count; i++) {
                result[i].renderable.transform.position = new Vector3(0, 0, -i);
            }

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

        //generalize
        //this is only for floors right now
        //more uses?
        private int UseTemplate(int templateId, int typeToMatch, int x, int y, int z) {
            int size = _tileMap.Size;
            int xl = x == 0 ? size - 1 : x - 1;
            int xr = x == size - 1 ? 0 : x + 1;
            int yu = y == 0 ? size - 1 : y + 1;
            int yd = y == size - 1 ? 0 : y - 1;

            bool above = _tileMap.GetTile(x, yu, z).Floor == typeToMatch;
            bool below = _tileMap.GetTile(x, yd, z).Floor == typeToMatch;
            bool left = _tileMap.GetTile(xl, y, z).Floor == typeToMatch;
            bool right = _tileMap.GetTile(xr, y, z).Floor == typeToMatch;

            int offset = templateId * 50;
            int v = (above ? 1 : 0) + (below ? 2 : 0) + (left ? 4 : 0) + (right ? 8 : 0);
            

            // this is where the complicated crap kicks in
            // i'd really like to replace this.
            // :'(
            // this is mostly guess work. I think. I wrote this code years ago. I know it works,
            // so I just copy and pasted. Shame on me.

            bool upleft = _tileMap.GetTile(xl, yu, z).Floor == typeToMatch;
            bool upright = _tileMap.GetTile(xr, yu, z).Floor == typeToMatch;
            bool downleft = _tileMap.GetTile(xl, yd, z).Floor == typeToMatch;
            bool downright = _tileMap.GetTile(xr, yd, z).Floor == typeToMatch;

            if (v == 15)
            {
                if (!upleft)
                {
                    v++;
                }
                if (!upright)
                {
                    v += 2;
                }
                if (!downleft)
                {
                    v += 4;
                }
                if (!downright)
                {
                    v += 8;
                }
            }
            else if (v == 5)
            {
                if (!upleft)
                {
                    v = 31;
                }
            }
            else if (v == 6)
            {
                if (!downleft)
                {
                    v = 32;
                }
            }
            else if (v == 9)
            {
                if (!upright)
                {
                    v = 33;
                }
            }
            else if (v == 10)
            {
                if (!downright)
                {
                    v = 34;
                }
            }
            else if (v == 7)
            {
                if (!downleft || !upleft)
                {
                    v = 34;
                    if (!downleft)
                    {
                        v++;
                    }
                    if (!upleft)
                    {
                        v += 2;
                    }
                }
            }
            else if (v == 11)
            {
                if (!downright || !upright)
                {
                    v = 37;
                    if (!downright)
                    {
                        v++;
                    }
                    if (!upright)
                    {
                        v += 2;
                    }
                }
            }
            else if (v == 13)
            {
                if (!upright || !upleft)
                {
                    v = 40;
                    if (!upright)
                    {
                        v++;
                    }
                    if (!upleft)
                    {
                        v += 2;
                    }
                }
            }
            else if (v == 14)
            {
                if (!downright || !downleft)
                {
                    v = 43;
                    if (!downright)
                    {
                        v++;
                    }
                    if (!downleft)
                    {
                        v += 2;
                    }
                }
            }

            return v + offset;
        }
    }

}