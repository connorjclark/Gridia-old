using System;
using System.Collections.Generic;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return items.Count; } }
        public int MonsterCount { get { return monsters.Count; } }
        
        private readonly List<Item> items;
        private readonly List<Monster> monsters;
        private readonly List<Animation> animations;
        private readonly List<ItemUse> uses;
        
        public ContentManager (String worldName)
        {
            items = new ContentLoader<Item>().Load(worldName + "/content/items.txt");
            monsters = new ContentLoader<Monster>().Load(worldName + "/content/monsters.txt");
            animations = new ContentLoader<Animation>().Load(worldName + "/content/animations.txt");
            uses = new ContentLoader<ItemUse>().Load(worldName + "/content/itemuses.txt");
        }

        public Item GetItem(int id)
        {
            var item = items[id];
            if (item == null) {
                return items[0];
            }
            return items[id];
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