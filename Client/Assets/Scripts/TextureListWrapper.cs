using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Serving.FileTransferring;

namespace Gridia
{
    public class TextureListWrapper {
        public List<Texture> Textures { get; private set; }
        public int Count { get { return Textures.Count; } }
        public String Prefix { get; private set; }
        public Texture FallbackTexture { get; private set; }
        private FileSystem _fileSystem;
        private List<int> _requestedTextureIndices = new List<int>();

        public TextureListWrapper(String prefix, Texture fallbackTexture, FileSystem fileSystem)
        {
            Prefix = prefix;
            FallbackTexture = fallbackTexture;
            _fileSystem = fileSystem;
            Textures = new List<Texture>();
        }

        public Texture GetTexture(int textureIndex) 
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

        public Texture GetTextureForSprite(int spriteIndex) 
        {
            var textureIndex = spriteIndex / GridiaConstants.SPRITES_IN_SHEET;
            return GetTexture(textureIndex);
        }

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
                        InsertTexture(tex, index);
                        _requestedTextureIndices.Remove(index);
                    });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }).Start();
        }

        private void InsertTexture(Texture texture, int index)
        {
            if (Count <= index)
            {
                for (var i = Count; i <= index; i++) 
                {
                    Textures.Add(null);
                }
            }
            Textures[index] = texture;
        }
    }
}
