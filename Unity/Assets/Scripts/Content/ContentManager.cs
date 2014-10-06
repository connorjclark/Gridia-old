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
        
        public ContentManager ()
        {
            items = new ContentLoader<Item>().Load("content/items");
            monsters = new ContentLoader<Monster>().Load("content/monsters");
            MonoBehaviour.print(monsters[121].Name);
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