using Serving.FileTransferring;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return items.Count; } }
        public int FloorCount { get { return floors.Count; } }
        public int MonsterCount { get { return monsters.Count; } }
        
        private readonly List<Item> items;
        private readonly List<Floor> floors;
        private readonly List<Monster> monsters;
        private readonly List<GridiaAnimation> animations;
        private readonly List<ItemUse> uses;
        private readonly FileSystem _fileSystem;
        
        public ContentManager(String worldName, FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            var clientDataFolder = @"worlds/" + worldName + @"/clientdata"; // :(
            items = Load<Item>(clientDataFolder + "/content/items.json");
            floors = Load<Floor>(clientDataFolder + "/content/floors.json");
            monsters = Load<Monster>(clientDataFolder + "/content/monsters.json");
            animations = Load<GridiaAnimation>(clientDataFolder + "/content/animations.json");
            uses = Load<ItemUse>(clientDataFolder + "/content/itemuses.json");
        }

        public ContentManager(String worldName)
            : this(worldName, new RegularFileSystem())
        {
        }

        private List<T> Load<T>(String filePath) where T : new()
        {
            var bytes = _fileSystem.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            return new ContentLoader<T>().Load(json);
        }

        public Item GetItem(int id)
        {
            var item = items[id];
            if (item == null) {
                return items[0];
            }
            return items[id];
        }

        public Floor GetFloor(int id)
        {
            return floors[id];
        }

        public Monster GetMonster(int id)
        {
            return monsters[id];
        }

        public GridiaAnimation GetAnimation(int id)
        {
            return animations[id - 1]; // :(
        }

        public List<ItemUse> GetUses(ItemInstance tool) 
        {
            return uses.FindAll(u => u.tool == tool.Item.Id);
        }
    }
}