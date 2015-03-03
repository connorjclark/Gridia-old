using System;
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

            var fallbackTexture = new Texture2D(320, 320)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            Floors = new TextureListWrapper(clientDataFolder + @"floors\floors", fallbackTexture, _fileSystem);
            Items = new TextureListWrapper(clientDataFolder + @"items\items", fallbackTexture, _fileSystem);
            Creatures = new TextureListWrapper(clientDataFolder + @"players\players", fallbackTexture, _fileSystem);
            Templates = new TextureListWrapper(clientDataFolder + @"templates\template", fallbackTexture, _fileSystem);
            Animations = new TextureListWrapper(clientDataFolder + @"animations\animation", fallbackTexture, _fileSystem);
            Heads = new TextureListWrapper(clientDataFolder + @"players\head", fallbackTexture, _fileSystem);
            Chests = new TextureListWrapper(clientDataFolder + @"players\chest", fallbackTexture, _fileSystem);
            Legs = new TextureListWrapper(clientDataFolder + @"players\legs", fallbackTexture, _fileSystem);
            Arms = new TextureListWrapper(clientDataFolder + @"players\arms", fallbackTexture, _fileSystem);
            Weapons = new TextureListWrapper(clientDataFolder + @"players\weapon", fallbackTexture, _fileSystem);
            Shields = new TextureListWrapper(clientDataFolder + @"players\shield", fallbackTexture, _fileSystem);

            DoneLoading = true;
        }

        private void DrawCreaturePart(Rect rect, TextureListWrapper textures, int spriteIndex)
        {
            var texture = textures.GetTextureForSprite(spriteIndex);
            var textureX = (spriteIndex % GridiaConstants.SpritesInSheet) % GridiaConstants.NumTilesInSpritesheetRow;
            var textureY = 9 - (spriteIndex % GridiaConstants.SpritesInSheet) / GridiaConstants.NumTilesInSpritesheetRow;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
        }

        public void DrawCreature(Rect rect, Creature creature, float scale)
        {
            // :(
            if (creature.IsOnRaft())
            {
                var cm = Locator.Get<ContentManager>();
                var raft = new ItemRenderable(Vector2.zero, cm.GetItem(1211).GetInstance()) {Rect = rect};
                raft.Render();
            }

            var image = creature.Image;
            if (image is DefaultCreatureImage)
            {
                var defaultImage = image as DefaultCreatureImage;
                var spriteId = defaultImage.SpriteIndex;
                var textureX = (spriteId % GridiaConstants.SpritesInSheet) % GridiaConstants.NumTilesInSpritesheetRow;
                var textureY = 10 - (spriteId % GridiaConstants.SpritesInSheet) / GridiaConstants.NumTilesInSpritesheetRow - defaultImage.Height; // ?
                var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, defaultImage.Width / 10.0f, defaultImage.Height / 10.0f); // :( don't hardcode 10
                rect.width *= defaultImage.Width;
                rect.height *= defaultImage.Height;
                rect.y -= (defaultImage.Height - 1) * GridiaConstants.SpriteSize * scale;
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