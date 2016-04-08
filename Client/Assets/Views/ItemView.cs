using Gridia;
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MarkUX.UnityProject
{
    public class ItemView : View
    {
        // TODO: investigate
        // property binding doesn't seem to work...this would be nicer:
        /*
        public ItemInstance Item {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                _spriteCache = GetSprites();
                _currentFrameIndex = -1;
            }
        }
        private ItemInstance _item;
        */

        public ItemInstance Item;

        public Views.Image Image;
        private int _currentFrameIndex;

        private List<Sprite> _spriteCache;
        private ItemInstance _spriteCacheItem; // because the above prop. binding doesn't work...

        public override void Initialize()
        {
            base.Initialize();
        }

        private List<Sprite> GetSprites()
        {
            var sprites = new List<Sprite>();
            var tm = Locator.Get<TextureManager>();
            if (Item.Item != null && Item.Item.Animations != null)
            {
                for (int i = 0; i < Item.Item.Animations.Length; i++)
                {
                    var sprite = tm.Items.GetSprite(Item.Item.Animations[i]);
                    sprites.Add(sprite);
                }
            }
            return sprites;
        }

        private void ReloadSprite()
        {
            if (_spriteCacheItem != Item)
            {
                _spriteCacheItem = Item;
                _spriteCache = GetSprites();
                _currentFrameIndex = -1;
            }

            if (_spriteCache == null || _spriteCache.Count() == 0)
            {
                return;
            }

            int frameIndex = (int)((Time.time * 6) % _spriteCache.Count()); //smell :(
            if (frameIndex != _currentFrameIndex)
            {
                _currentFrameIndex = frameIndex;
                SetValue(() => Image.BackgroundImage, _spriteCache[frameIndex]);
            }
        }

        public void Update()
        {
            if (Item != null)
            {
                ReloadSprite();
            }
        }
    }
}
