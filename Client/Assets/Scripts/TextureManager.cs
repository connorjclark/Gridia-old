using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gridia
{
    public class TextureManager
    {
        public List<Texture> Floors { get; private set; }
        public List<Texture> Items { get; private set; }
        public List<Texture> Creatures { get; private set; }
        public List<Texture> Templates { get; private set; }
        public List<Texture> Animations { get; private set; }
        public List<Texture> Heads { get; private set; }
        public List<Texture> Chests { get; private set; }
        public List<Texture> Legs { get; private set; }
        public List<Texture> Arms { get; private set; }
        public List<Texture> Weapons { get; private set; }
        public List<Texture> Shields { get; private set; }

        public TextureManager (String worldName)
        {
            var clientDataFolder = @"worlds\" + worldName + @"\clientdata"; // :(
            Floors = LoadTextures(clientDataFolder + "/floors/floors", 6); // :(
            Items = LoadTextures(clientDataFolder + "/items/items", 27);
            Creatures = LoadTextures(clientDataFolder + "/players/players", 8);
            Templates = LoadTextures(clientDataFolder + "/templates/template", 1);
            Animations = LoadTextures(clientDataFolder + "/animations/animation", 2);
            Heads = LoadTextures(clientDataFolder + "/players/head", 2);
            Chests = LoadTextures(clientDataFolder + "/players/chest", 1);
            Legs = LoadTextures(clientDataFolder + "/players/legs", 1);
            Arms = LoadTextures(clientDataFolder + "/players/arms", 1);
            Weapons = LoadTextures(clientDataFolder + "/players/weapon", 1);
            Shields = LoadTextures(clientDataFolder + "/players/shield", 1);
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