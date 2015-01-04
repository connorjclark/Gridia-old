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
        private List<Texture> Heads { get; set; }
        private List<Texture> Chests { get; set; }
        private List<Texture> Legs { get; set; }
        private List<Texture> Arms { get; set; }
        private List<Texture> Weapons { get; set; }
        private List<Texture> Shields { get; set; }

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
            Weapons = LoadTextures(clientDataFolder + "/players/weapon", 2);
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

        private void DrawCreaturePart(Rect rect, List<Texture> textures, int spriteIndex)
        {
            int textureIndex = spriteIndex / GridiaConstants.SPRITES_IN_SHEET;
            if (textureIndex >= textures.Count)
            {
                textureIndex = 0;
            }
            var texture = textures[textureIndex];
            int textureX = (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
        }

        public void DrawCreature(Rect rect, Creature creature, float scale)
        {
            // :(
            if (creature.IsOnRaft())
            {
                var cm = Locator.Get<ContentManager>();
                var raft = new ItemRenderable(Vector2.zero, cm.GetItem(1211).GetInstance());
                raft.Rect = rect;
                raft.Render();
            }

            var image = creature.Image;
            if (image is DefaultCreatureImage)
            {
                var defaultImage = image as DefaultCreatureImage;
                int spriteId = defaultImage.SpriteIndex;
                int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
                int textureY = 10 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW - defaultImage.Height; // ?
                var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, defaultImage.Width / 10.0f, defaultImage.Height / 10.0f); // :( don't hardcode 10
                rect.width *= defaultImage.Width;
                rect.height *= defaultImage.Height;
                rect.y -= (defaultImage.Height - 1) * GridiaConstants.SPRITE_SIZE * scale;
                GUI.DrawTextureWithTexCoords(rect, Creatures[spriteId / GridiaConstants.SPRITES_IN_SHEET], texCoords);
            }
            else if (image is CustomPlayerImage)
            {
                var customImage = image as CustomPlayerImage;
                DrawCreaturePart(rect, Heads, customImage.Head);
                DrawCreaturePart(rect, Chests, customImage.Chest);
                DrawCreaturePart(rect, Legs, customImage.Legs);
                DrawCreaturePart(rect, Arms, customImage.Arms);
                DrawCreaturePart(rect, Weapons, customImage.Weapon);
                DrawCreaturePart(rect, Shields, customImage.Shield);
            }
        }
    }
}