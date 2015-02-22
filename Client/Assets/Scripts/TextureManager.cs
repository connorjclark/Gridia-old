using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Serving.FileTransferring;

namespace Gridia
{
    public class TextureManager
    {
        public TextureListWrapper Floors { get; private set; }
        public TextureListWrapper Items { get; private set; }
        public TextureListWrapper Creatures { get; private set; }
        public TextureListWrapper Templates { get; private set; }
        public TextureListWrapper Animations { get; private set; }
        private TextureListWrapper Heads { get; set; }
        private TextureListWrapper Chests { get; set; }
        private TextureListWrapper Legs { get; set; }
        private TextureListWrapper Arms { get; set; }
        private TextureListWrapper Weapons { get; set; }
        private TextureListWrapper Shields { get; set; }
        public bool DoneLoading { get; private set; }
        private readonly FileSystem _fileSystem;

        public TextureManager(String worldName)
        {
            _fileSystem = GridiaConstants.GetFileSystem();
            Initiate(worldName);
        }

        private void Initiate(String worldName) {
            var clientDataFolder = @"worlds\" + worldName + @"\clientdata\"; // :(

            var fallbackTexture = new Texture2D(320, 320);
            fallbackTexture.filterMode = FilterMode.Point;
            fallbackTexture.wrapMode = TextureWrapMode.Clamp;

            var lists = new TextureListWrapper[]
            {
                Floors = new TextureListWrapper(clientDataFolder + @"floors\floors", fallbackTexture, _fileSystem),
                Items = new TextureListWrapper(clientDataFolder + @"items\items", fallbackTexture, _fileSystem),
                Creatures = new TextureListWrapper(clientDataFolder + @"players\players", fallbackTexture, _fileSystem),
                Templates = new TextureListWrapper(clientDataFolder + @"templates\template", fallbackTexture, _fileSystem),
                Animations = new TextureListWrapper(clientDataFolder + @"animations\animation", fallbackTexture, _fileSystem),
                Heads = new TextureListWrapper(clientDataFolder + @"players\head", fallbackTexture, _fileSystem),
                Chests = new TextureListWrapper(clientDataFolder + @"players\chest", fallbackTexture, _fileSystem),
                Legs = new TextureListWrapper(clientDataFolder + @"players\legs", fallbackTexture, _fileSystem),
                Arms = new TextureListWrapper(clientDataFolder + @"players\arms", fallbackTexture, _fileSystem),
                Weapons = new TextureListWrapper(clientDataFolder + @"players\weapon", fallbackTexture, _fileSystem),
                Shields = new TextureListWrapper(clientDataFolder + @"players\shield", fallbackTexture, _fileSystem)
            };

            /*var numTextures = new int[] {
                6,
                27,
                8,
                1,
                2,
                2,
                1,
                1,
                1,
                2,
                1
            };*/ // :(

            /*for (var i = 0; i < lists.Length; i++) {
                var list = lists[i];
                var numTexture = numTextures[i];
                LoadTextures(list.Textures, "", numTexture);
            }*/
            DoneLoading = true;
        }

        private void LoadTextures(List<Texture> into, String prefix, int numTextures) {
            for (var i = 0; i < numTextures; i++ )
            {
                var data = _fileSystem.ReadAllBytes(prefix + i + ".png");
                MainThreadQueue.Add(() => {
                    var tex = new Texture2D(320, 320);
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.LoadImage(data);
                    into.Add(tex);
                });
            }
        }

        private void DrawCreaturePart(Rect rect, TextureListWrapper textures, int spriteIndex)
        {
            int textureIndex = spriteIndex / GridiaConstants.SPRITES_IN_SHEET;
            if (textureIndex >= textures.Count)
            {
                textureIndex = 0;
            }
            var texture = textures.GetTexture(textureIndex);
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
                GUI.DrawTextureWithTexCoords(rect, Creatures.GetTextureForSprite(spriteId), texCoords);
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