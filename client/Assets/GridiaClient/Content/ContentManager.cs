namespace Gridia
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    using Serving.FileTransferring;

    public class ContentManager
    {
        #region Fields

        private readonly FileSystem _fileSystem;

        private List<GridiaAnimation> _animations;
        private List<Floor> _floors;
        private List<Item> _items;
        private List<Monster> _monsters;
        private List<ItemUse> _uses;

        #endregion Fields

        #region Constructors

        public ContentManager(String worldName)
        {
            _fileSystem = GridiaConstants.GetFileSystem();
            var clientDataFolder = @"worlds/" + worldName + @"/clientdata"; // :(

            if (!_fileSystem.DirectoryExists(clientDataFolder) && _fileSystem.DirectoryExists("../" + clientDataFolder))
            {
              clientDataFolder = @"../" + clientDataFolder;
            }

            _items = Load<Item>(clientDataFolder + "/content/items.json");
            _floors = Load<Floor>(clientDataFolder + "/content/floors.json");
            _monsters = Load<Monster>(clientDataFolder + "/content/monsters.json");
            _animations = Load<GridiaAnimation>(clientDataFolder + "/content/animations.json");
            _uses = Load<ItemUse>(clientDataFolder + "/content/itemuses.json");
        }

        #endregion Constructors

        #region Properties

        public bool DoneLoading
        {
            get; private set;
        }

        public int FloorCount
        {
            get { return _floors.Count; }
        }

        public int ItemCount
        {
            get { return _items.Count; }
        }

        public int MonsterCount
        {
            get { return _monsters.Count; }
        }

        #endregion Properties

        #region Methods

        public GridiaAnimation GetAnimation(string name)
        {
            return _animations.Find(anim => String.Equals(anim.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public Floor GetFloor(int id)
        {
            return _floors[id];
        }

        public Item GetItem(int id)
        {
            var item = _items[id];
            return item == null ? _items[0] : _items[id];
        }

        public Monster GetMonster(int id)
        {
            return _monsters[id];
        }

        public List<ItemUse> GetUses(ItemInstance tool)
        {
            return _uses.FindAll(u => u.Tool == tool.Item.Id);
        }

        private List<T> Load<T>(String filePath)
            where T : new()
        {
            var bytes = _fileSystem.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            return new ContentLoader<T>().Load(json);
        }

        #endregion Methods
    }
}