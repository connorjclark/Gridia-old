using System;
using System.Collections.Generic;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return items.Count; } }
        public int FloorCount { get { return 300; } } // :(
        public int MonsterCount { get { return monsters.Count; } }
        
        private readonly List<Item> items;
        private readonly List<Floor> floors;
        private readonly List<Monster> monsters;
        private readonly List<Animation> animations;
        private readonly List<ItemUse> uses;
        
        public ContentManager (String worldName)
        {
            items = new ContentLoader<Item>().Load(worldName + "/content/items.json");
            floors = new ContentLoader<Floor>().Load(worldName + "/content/floors.json");
            monsters = new ContentLoader<Monster>().Load(worldName + "/content/monsters.json");
            animations = new ContentLoader<Animation>().Load(worldName + "/content/animations.json");
            uses = new ContentLoader<ItemUse>().Load(worldName + "/content/itemuses.json");
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

        public Animation GetAnimation(int id)
        {
            return animations[id - 1]; // :(
        }

        public List<ItemUse> GetUses(ItemInstance tool) 
        {
            return uses.FindAll(u => u.tool == tool.Item.Id);
        }
    }
}