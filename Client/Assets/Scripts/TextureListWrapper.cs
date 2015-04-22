using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Serving.FileTransferring;

namespace Gridia
{
    public class TextureListWrapper {
        public List<Texture2D> Textures { get; private set; }
        public int Count { get { return Textures.Count; } }
        public String Prefix { get; private set; }
        public Texture2D FallbackTexture { get; private set; }
        private readonly FileSystem _fileSystem;
        private readonly List<int> _requestedTextureIndices = new List<int>();

        public TextureListWrapper(String prefix, Texture2D fallbackTexture, FileSystem fileSystem)
        {
            Prefix = prefix;
            FallbackTexture = fallbackTexture;
            _fileSystem = fileSystem;
            Textures = new List<Texture2D>();
        }

        public Sprite GetSprite(int spriteIndex, int width = 1, int height = 1)
        {
            var tex = GetTextureForSprite(spriteIndex);
            if (tex == FallbackTexture) return null;
            var x = (spriteIndex%GridiaConstants.SpritesInSheet)%GridiaConstants.NumTilesInSpritesheetRow;
            var y = 10 - (spriteIndex%GridiaConstants.SpritesInSheet)/GridiaConstants.NumTilesInSpritesheetRow - height; // ?
            return Sprite.Create(tex, new Rect(x*32, y*32, 32*width, 32*height), new Vector2(0.5f, 0.5f), 1);
        }

        public Texture2D GetTexture(int textureIndex) 
        {
            if (Count <= textureIndex || Textures[textureIndex] == null)
            {
                if (!_requestedTextureIndices.Contains(textureIndex))
                {
                    LoadTexture(textureIndex);
                }
                return FallbackTexture;
            }
            return Textures[textureIndex];
        }

        public Texture2D GetTextureForSprite(int spriteIndex) 
        {
            var textureIndex = spriteIndex / GridiaConstants.SpritesInSheet;
            return GetTexture(textureIndex);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void LoadTexture(int index)
        {
            _requestedTextureIndices.Add(index);
            new Thread(() =>
            {
                try
                {
                    var path = Prefix + index + ".png";
                    Debug.Log("Loading texture: " + path);
                    var data = _fileSystem.ReadAllBytes(path);
                    MainThreadQueue.Add(() =>
                    {
                        var tex = new Texture2D(320, 320)
                        {
                            filterMode = FilterMode.Point,
                            wrapMode = TextureWrapMode.Clamp
                        };
                        tex.LoadImage(data);
                        InsertIntoList(Textures, tex, index);
                        _requestedTextureIndices.Remove(index);
                    });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }).Start();
        }

        private void InsertIntoList<T>(List<T> list, T texture, int index)
        {
            if (list.Count <= index)
            {
                for (var i = list.Count; i <= index; i++) 
                {
                    Textures.Add(null);
                }
            }
            list[index] = texture;
        }
    }
}
