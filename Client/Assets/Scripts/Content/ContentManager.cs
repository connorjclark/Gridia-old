using Serving.FileTransferring;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return items.Count; } }
        public int FloorCount { get { return floors.Count; } }
        public int MonsterCount { get { return monsters.Count; } }
        public bool DoneLoading { get; private set; }
        
        private List<Item> items;
        private List<Floor> floors;
        private List<Monster> monsters;
        private List<GridiaAnimation> animations;
        private List<ItemUse> uses;
        private FileSystem _fileSystem;
        
        public ContentManager(String worldName)
        {
            _fileSystem = GridiaConstants.GetFileSystem();
            new Thread(() => {
                var clientDataFolder = @"worlds/" + worldName + @"/clientdata"; // :(
                items = Load<Item>(clientDataFolder + "/content/items.json");
                floors = Load<Floor>(clientDataFolder + "/content/floors.json");
                monsters = Load<Monster>(clientDataFolder + "/content/monsters.json");
                animations = Load<GridiaAnimation>(clientDataFolder + "/content/animations.json");
                uses = Load<ItemUse>(clientDataFolder + "/content/itemuses.json");
                DoneLoading = true;
            }).Start();
        }

        private List<T> Load<T>(String filePath) where T : new()
        {
            var bytes = _fileSystem.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            //var json = _fileSystem.ReadString(filePath);
            //UnityEngine.Debug.Log("json = " + json); // :(
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