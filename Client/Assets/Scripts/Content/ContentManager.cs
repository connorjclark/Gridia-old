using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Gridia
{
    public class ContentManager
    {
        public int ItemCount { get { return items.Count; } }
        public int MonsterCount { get { return monsters.Count; } }
        
        private readonly List<Item> items;
        private readonly List<Monster> monsters;
        
        public ContentManager (String worldName)
        {
            items = new ContentLoader<Item>().Load(worldName + "/content/items.txt");
            monsters = new ContentLoader<Monster>().Load(worldName + "/content/monsters.txt");
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