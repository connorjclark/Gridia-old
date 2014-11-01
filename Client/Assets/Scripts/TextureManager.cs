using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gridia
{
    public class TextureManager
    {
        private List<Texture> floors = new List<Texture> ();
        private List<Texture> items = new List<Texture> ();
        private List<Texture> creatures = new List<Texture> ();

        public TextureManager (String worldName)
        {
            floors = LoadTextures(worldName + "/floors/floors", 1);
            items = LoadTextures(worldName + "/items/items", 27);
            creatures = LoadTextures(worldName + "/players/players", 8);
        }
            
        public Texture GetFloorsTexture (int index)
        {
            return floors [index];
        }
            
        public Texture GetItemsTexture (int index)
        {
            return items[index];
        }

        public Texture GetCreaturesTexture (int index)
        {
            return creatures [index];
        }

        private List<Texture> LoadTextures(String prefix, int numTextures)
        {
            var textures = new List<Texture>();
            for (var i = 0; i < numTextures; i++ )
            {
                byte[] data = File.ReadAllBytes(prefix + i + ".png");
                var tex = new Texture2D(320, 320);
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.LoadImage(data);
                textures.Add(tex);
            }
            return textures;
        }
    }
}