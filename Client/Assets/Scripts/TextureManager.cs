using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class TextureManager
    {
        private List<Texture> floors = new List<Texture> ();
        private List<Texture> items = new List<Texture> ();
        private List<Texture> creatures = new List<Texture> ();

        public TextureManager ()
        {
            floors = LoadTextures ("floors/floors", 1);
            items = LoadTextures ("items/items", 27);
            creatures = LoadTextures ("players/players", 8);
        }
            
        public Texture GetFloorsTexture (int index)
        {
            return floors [index];
        }
            
        public Texture GetItemsTexture (int index)
        {
            return items [index];
        }

        public Texture GetCreaturesTexture (int index)
        {
            return creatures [index];
        }
          
        //load lazily?
        private List<Texture> LoadTextures (string prefix, int numTextures)
        {
            List<Texture> textures = new List<Texture> ();
            for (int i = 0; i < numTextures; i++) {
                Texture texture = Resources.Load (prefix + i) as Texture;
                textures.Add (texture);
            }
            return textures;
        }
    }
}