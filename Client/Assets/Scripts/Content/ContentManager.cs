using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Gridia
{
    public class ContentManager
    {
        public static ContentManager Singleton { get; set; } //smell?

        public int ItemCount { get { return items.Count; } }
        public int MonsterCount { get { return monsters.Count; } }
        
        private readonly List<Item> items;
        private readonly List<Monster> monsters;
        
        public ContentManager (String worldName)
        {
            items = new ContentLoader<Item>().Load(worldName + "/content/items");
            monsters = new ContentLoader<Monster>().Load(worldName + "content/monsters");
            Singleton = this; //smell?
        }

        public Item GetItem(int id)
        {
            return items[id];
        }

        public Monster GetMonster(int id)
        {
            return monsters[id];
        }
    }
}