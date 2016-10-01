namespace Gridia
{
    using System;

    using Serving.FileTransferring;

    using UnityEngine;

    public class TextureManager
    {
        #region Fields

        private readonly FileSystem _fileSystem;

        #endregion Fields

        #region Constructors

        public TextureManager(String worldName)
        {
            _fileSystem = GridiaConstants.GetFileSystem();
            Initiate(worldName);

            // TODO: profile if lazily loading textures is worth it
            // if there isn't much benefit, just load all at once
            LoadAll();
        }

        #endregion Constructors

        #region Properties

        public TextureListWrapper Animations
        {
            get; private set;
        }

        public TextureListWrapper Creatures
        {
            get; private set;
        }

        public TextureListWrapper Floors
        {
            get; private set;
        }

        public TextureListWrapper Items
        {
            get; private set;
        }

        public TextureListWrapper Templates
        {
            get; private set;
        }

        private TextureListWrapper Arms
        {
            get; set;
        }

        private TextureListWrapper Chests
        {
            get; set;
        }

        private TextureListWrapper Heads
        {
            get; set;
        }

        private TextureListWrapper Legs
        {
            get; set;
        }

        private TextureListWrapper Shields
        {
            get; set;
        }

        private TextureListWrapper Weapons
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        // :( delete
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
                var defaultImage = (DefaultCreatureImage) image;
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
                var customImage = (CustomPlayerImage) image;
                DrawCreaturePart(rect, Heads, customImage.Head);
                DrawCreaturePart(rect, Chests, customImage.Chest);
                DrawCreaturePart(rect, Legs, customImage.Legs);
                DrawCreaturePart(rect, Arms, customImage.Arms);
                DrawCreaturePart(rect, Weapons, customImage.Weapon);
                DrawCreaturePart(rect, Shields, customImage.Shield);
            }
        }

        public void LoadAll()
        {
            Floors.LoadAll();
            Items.LoadAll();
            Creatures.LoadAll();
            Templates.LoadAll();
            Animations.LoadAll();
            Heads.LoadAll();
            Chests.LoadAll();
            Legs.LoadAll();
            Arms.LoadAll();
            Weapons.LoadAll();
            Shields.LoadAll();
        }

        private void DrawCreaturePart(Rect rect, TextureListWrapper textures, int spriteIndex)
        {
            var texture = textures.GetTextureForSprite(spriteIndex);
            var textureX = (spriteIndex % GridiaConstants.SpritesInSheet) % GridiaConstants.NumTilesInSpritesheetRow;
            var textureY = 9 - (spriteIndex % GridiaConstants.SpritesInSheet) / GridiaConstants.NumTilesInSpritesheetRow;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
        }

        private void Initiate(String worldName)
        {
            var clientDataFolder = @"worlds\" + worldName + @"\clientdata\"; // :(

            if (!_fileSystem.DirectoryExists(clientDataFolder) && _fileSystem.DirectoryExists("../" + clientDataFolder))
            {
              clientDataFolder = @"../" + clientDataFolder;
            }

            Floors = new TextureListWrapper(clientDataFolder + @"floors\floors", _fileSystem);
            Items = new TextureListWrapper(clientDataFolder + @"items\items", _fileSystem);
            Creatures = new TextureListWrapper(clientDataFolder + @"players\players", _fileSystem);
            Templates = new TextureListWrapper(clientDataFolder + @"templates\template", _fileSystem);
            Animations = new TextureListWrapper(clientDataFolder + @"animations\animation", _fileSystem);
            Heads = new TextureListWrapper(clientDataFolder + @"players\head", _fileSystem);
            Chests = new TextureListWrapper(clientDataFolder + @"players\chest", _fileSystem);
            Legs = new TextureListWrapper(clientDataFolder + @"players\legs", _fileSystem);
            Arms = new TextureListWrapper(clientDataFolder + @"players\arms", _fileSystem);
            Weapons = new TextureListWrapper(clientDataFolder + @"players\weapon", _fileSystem);
            Shields = new TextureListWrapper(clientDataFolder + @"players\shield", _fileSystem);
        }

        #endregion Methods
    }
}