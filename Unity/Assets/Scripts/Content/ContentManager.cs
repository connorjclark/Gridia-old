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
        private readonly List<Item> items;
        
        public ContentManager ()
        {
            items = new ItemsLoader ().Load ("content/items");
            Singleton = this; //smell?
        }

        public Item GetItem (int id)
        {
            return items [id];
        }
    }
}