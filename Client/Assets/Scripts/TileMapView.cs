using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gridia
{
    public class TileMapView
    {
        public static int OffGridTiles = 400; // stress test this limit

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
                return pos - new Vector3(Width / 2, Height / 2, 0);
            }
        }
        public bool IsLighting { get; set; }
        public int NumGridTiles { get { return (Width + 2) * (Height + 2); } }
        public int NumTiles { get { return NumGridTiles + OffGridTiles; } }
        public float TileSize { get { return GridiaConstants.SpriteSize * _scale; } }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                Initialize();
            }
        }

        private readonly TileMap _tileMap;
        private readonly TextureManager _textureManager;
        private List<Layer> _layers;
        private readonly Lighting _lighting;
        private readonly Shader _shader;
        private Vector3[] _gridVertices;
        public int Width, Height;
        private float _scale;

        public TileMapView(TileMap tileMap, TextureManager textureManager, float scale = 1.0f)
        {
            _tileMap = tileMap;
            _textureManager = textureManager;
            _shader = FindShader();
            Scale = scale;
            _lighting = new Lighting(this);
            IsLighting = false;
        }

        public void Initialize() 
        {
            var tileSize = GridiaConstants.SpriteSize * _scale;
            Width = Mathf.CeilToInt(Screen.width / tileSize);
            Height = Mathf.CeilToInt(Screen.height / tileSize);
            InitGridVertices();
            if (_layers != null)
            {
                _layers.ForEach(layer =>
                {
                    layer.Delete();
                });
            }
            _layers = InitLayers();
        }

        private int _prevZ; // :(

        public void Render()
        {
            if (_prevZ != FocusPosition.z)
            {
                Initialize();
                _prevZ = (int)FocusPosition.z;
            }

            Action<int[], int, int> setTriangles = (triangles, offset, tileIndex) =>
            {
                var vertexOffset = tileIndex * 4;
                triangles[offset + 0] = vertexOffset;
                triangles[offset + 1] = vertexOffset + 1;
                triangles[offset + 2] = vertexOffset + 2;
                triangles[offset + 3] = vertexOffset;
                triangles[offset + 4] = vertexOffset + 2;
                triangles[offset + 5] = vertexOffset + 3;
            };

            Action<Vector2[], int, int, int, int, int> setTextureCoords = (uv, tileIndex, tileX, tileY, width, height) =>
            {
                var offset = tileIndex * 4;

                var x1 = tileX * GridiaConstants.SpriteUv;
                var y1 = 1.0f - (tileY + height) * GridiaConstants.SpriteUv; //smell
                var x2 = x1 + GridiaConstants.SpriteUv * width;
                var y2 = y1 + GridiaConstants.SpriteUv * height;

                uv[offset + 0].Set(x1, y1);
                uv[offset + 1].Set(x1, y2);
                uv[offset + 2].Set(x2, y2);
                uv[offset + 3].Set(x2, y1);
            };

            Action<Layer> drawLayer = layer =>
            {
                var triangleBatches = new List<int[]>();
                var textureBatches = new List<Texture>();
                var elementList = new List<Dictionary<string, object>>();

                var tileIndex = 0;
                var pos = FocusPosition;
                var positionX = (int) pos.x;
                var positionY = (int) pos.y;
                var positionZ = (int) pos.z;
                var numTilesOffGrid = 0;

                ForEachInView((x, y) =>
                {
                    var tile = _tileMap.GetTile(x + positionX, y + positionY, positionZ);
                    var dataIndex = layer.GetTileData(tile);
                    if (dataIndex != -1)
                    {
                        Texture textureForThisData;

                        //hack for template :(
                        if (dataIndex <= -2)
                        {
                            var templateId = dataIndex + 3;
                            var typeToMatch = templateId == 0 ? 1 : 0;
                            textureForThisData = _textureManager.Templates.GetTexture(0);
                            dataIndex = UseTemplate(templateId, typeToMatch, x + positionX, y + positionY, positionZ);
                        }
                        else
                        {
                            textureForThisData = layer.GetTexture(dataIndex);
                        }

                        var tileX = (dataIndex % 100) % GridiaConstants.NumTilesInSpritesheetRow;
                        var tileY = (dataIndex % 100) / GridiaConstants.NumTilesInSpritesheetRow;
                        var dic = new Dictionary<string, object>();
                        dic["texture"] = textureForThisData;

                        var width = layer.GetWidth(tile);
                        var height = layer.GetHeight(tile);

                        var tileOffset = layer.GetOffset(tile);
                        if ((tileOffset != Vector2.zero || width != 1 || height != 1) && numTilesOffGrid < OffGridTiles)
                        {
                            var offTileIndex = NumGridTiles + numTilesOffGrid;
                            SetTileVertices(offTileIndex * 4, x + tileOffset.x, y + tileOffset.y, width, height);
                            setTextureCoords(layer.uv, offTileIndex, tileX, tileY, width, height);
                            dic["tileIndex"] = offTileIndex;
                            numTilesOffGrid++;
                        }
                        else
                        {
                            setTextureCoords(layer.uv, tileIndex, tileX, tileY, 1, 1);
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
                    var tileSheet1Hash = e1["texture"].GetHashCode();
                    var tileSheet2Hash = e2["texture"].GetHashCode();
                    return tileSheet1Hash == tileSheet2Hash ? 0 : (tileSheet1Hash > tileSheet2Hash ? 1 : -1);
                });

                Action<Texture, int, int> drawBatch = (texture, count, offset) =>
                {
                    var tri = new int[count * 6];
                    for (var i = 0; i < count; i++)
                    {
                        setTriangles(tri, i * 6, (int)elementList[i + offset]["tileIndex"]);
                    }

                    triangleBatches.Add(tri);
                    textureBatches.Add(texture);
                };

                var offsetForNextBatch = 0;
                for (var i = 1; i < elementList.Count; i++)
                {
                    var currentElement = elementList[i];
                    var prevElement = elementList[i - 1];
                    var currentTexture = currentElement["texture"] as Texture;
                    var prevTexture = prevElement["texture"] as Texture;
                    if (currentTexture != prevTexture)
                    {
                        var count = (i - offsetForNextBatch) * 6;
                        var offset = offsetForNextBatch * 6 * 2;

                        count = i - offsetForNextBatch;
                        offset = offsetForNextBatch;

                        drawBatch(prevTexture, count, offset);
                        offsetForNextBatch = i;
                    }
                }

                drawBatch((Texture)elementList[elementList.Count - 1]["texture"], (elementList.Count - offsetForNextBatch), offsetForNextBatch);

                layer.Mesh.vertices = _gridVertices;

                layer.ApplyUV();

                var materials = new Material[textureBatches.Count];

                for (var i = 0; i < triangleBatches.Count; i++)
                {
                    var material = new Material(_shader) {mainTexture = textureBatches[i]};
                    materials[i] = material;
                }

                layer.Renderable.renderer.materials = materials;

                layer.Mesh.subMeshCount = triangleBatches.Count;
                for (var i = 0; i < triangleBatches.Count; i++)
                {
                    layer.Mesh.SetTriangles(triangleBatches[i], i);
                }

                var layerz = layer.Renderable.transform.position.z;
                var renderablePosition = TileSize * Utilities.Vector3Residual(-pos);
                var newLayerPos = Utilities.Vector3Floor(renderablePosition);
                newLayerPos.z = layerz; // :(
                layer.Renderable.transform.position = newLayerPos;
            };

            _layers.ForEach(drawLayer);

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
            var lights = new List<Vector3>();
            var posX = (int) Focus.Position.x;
            var posY = (int) Focus.Position.y;
            var posZ = (int) Focus.Position.z;
            ForEachInView((x, y) =>
            {
                var tile = _tileMap.GetTile(x + posX, y + posY, posZ);
                var lightIntensity = tile.Item.Item.Light;
                if (lightIntensity != 0)
                {
                    lights.Add(new Vector3(x, y, lightIntensity));
                }
            });
            _lighting.SetLights(lights);
        }

        // :(
        public void ForEachInView(Action<int, int> task)
        {
            for (var y = -1; y < Height + 1; y++)
            {
                for (var x = -1; x < Width + 1; x++)
                {
                    task(x, y);
                }
            }
        }

        private List<Layer> InitLayers()
        {
            var result = new List<Layer>();

            var contentManager = Locator.Get<ContentManager>();

            result.Add(new Layer(
                "Floor layer",
                this,
                tile =>
                {
                    switch (tile.Floor)
                    {
                        case 0:
                            return -2;
                        case 1:
                            return -3;
                    }
                    return tile.Floor;
                },
                data => _textureManager.Floors.GetTextureForSprite(data)
            ));

            result.Add(new Layer(
                "Item layer", 
                this, 
                tile =>
                {
                    var animations = tile.Item.Item.Animations;
                    if (animations == null || animations.Length == 0)
                    {
                        return -1;
                    }
                    var frame = (int)((Time.time * 6) % animations.Length);//smell :(
                    return animations[frame];
                },
                data => _textureManager.Items.GetTextureForSprite(data),
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
                },
                tile => tile.Item.Item.ImageWidth,
                tile => tile.Item.Item.ImageHeight
            ));

            for (var i = 0; i < result.Count; i++)
            {
                result[i].Renderable.transform.position = new Vector3(0, 0, -i);
            }

            return result;
        }

        private void InitGridVertices()
        {
            _gridVertices = new Vector3[NumTiles * 4];
            var offset = 0;
            ForEachInView((x, y) =>
            {
                SetTileVertices(offset, x, y, 1, 1);
                offset += 4;
            });
        }

        private void SetTileVertices(int offset, float x, float y, int width, int height) {
            var x1 = x * TileSize;
            var y1 = y * TileSize;
            var x2 = x1 + TileSize * width;
            var y2 = y1 + TileSize * height;
            _gridVertices[offset + 0] = new Vector3(x1, y1, 0);
            _gridVertices[offset + 1] = new Vector3(x1, y2, 0);
            _gridVertices[offset + 2] = new Vector3(x2, y2, 0);
            _gridVertices[offset + 3] = new Vector3(x2, y1, 0);
        }

        //generalize
        //this is only for floors right now
        //more uses?
        private int UseTemplate(int templateId, int typeToMatch, int x, int y, int z) {
            var size = _tileMap.Size;
            var xl = x == 0 ? size - 1 : x - 1;
            var xr = x == size - 1 ? 0 : x + 1;
            var yu = y == 0 ? size - 1 : y + 1;
            var yd = y == size - 1 ? 0 : y - 1;

            var above = _tileMap.GetTile(x, yu, z).Floor == typeToMatch;
            var below = _tileMap.GetTile(x, yd, z).Floor == typeToMatch;
            var left = _tileMap.GetTile(xl, y, z).Floor == typeToMatch;
            var right = _tileMap.GetTile(xr, y, z).Floor == typeToMatch;

            var offset = templateId * 50;
            var v = (above ? 1 : 0) + (below ? 2 : 0) + (left ? 4 : 0) + (right ? 8 : 0);
            

            // this is where the complicated crap kicks in
            // i'd really like to replace this.
            // :'(
            // this is mostly guess work. I think. I wrote this code years ago. I know it works,
            // so I just copy and pasted. Shame on me.

            var upleft = _tileMap.GetTile(xl, yu, z).Floor == typeToMatch;
            var upright = _tileMap.GetTile(xr, yu, z).Floor == typeToMatch;
            var downleft = _tileMap.GetTile(xl, yd, z).Floor == typeToMatch;
            var downright = _tileMap.GetTile(xr, yd, z).Floor == typeToMatch;

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