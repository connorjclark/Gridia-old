using Serving.FileTransferring;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return _items.Count; } }
        public int FloorCount { get { return _floors.Count; } }
        public int MonsterCount { get { return _monsters.Count; } }
        public bool DoneLoading { get; private set; }
        
        private List<Item> _items;
        private List<Floor> _floors;
        private List<Monster> _monsters;
        private List<GridiaAnimation> _animations;
        private List<ItemUse> _uses;
        private readonly FileSystem _fileSystem;
        
        public ContentManager(String worldName)
        {
            _fileSystem = GridiaConstants.GetFileSystem();
            new Thread(() => {
                var clientDataFolder = @"worlds/" + worldName + @"/clientdata"; // :(
                _items = Load<Item>(clientDataFolder + "/content/items.json");
                _floors = Load<Floor>(clientDataFolder + "/content/floors.json");
                _monsters = Load<Monster>(clientDataFolder + "/content/monsters.json");
                _animations = Load<GridiaAnimation>(clientDataFolder + "/content/animations.json");
                _uses = Load<ItemUse>(clientDataFolder + "/content/itemuses.json");
                DoneLoading = true;
            }).Start();
        }

        private List<T> Load<T>(String filePath) where T : new()
        {
            var bytes = _fileSystem.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            return new ContentLoader<T>().Load(json);
        }

        public Item GetItem(int id)
        {
            var item = _items[id];
            return item == null ? _items[0] : _items[id];
        }

        public Floor GetFloor(int id)
        {
            return _floors[id];
        }

        public Monster GetMonster(int id)
        {
            return _monsters[id];
        }

        public GridiaAnimation GetAnimation(int id)
        {
            return _animations[id - 1]; // :(
        }

        public List<ItemUse> GetUses(ItemInstance tool) 
        {
            return _uses.FindAll(u => u.Tool == tool.Item.Id);
        }
    }
}