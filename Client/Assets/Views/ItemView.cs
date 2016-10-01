namespace MarkLight.UnityProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gridia;

    using MarkLight;
    using MarkLight.ValueConverters;
    using MarkLight.Views;
    using MarkLight.Views.UI;

    using UnityEngine;

    public class ItemView : UIView
    {
        #region Fields

        public Views.UI.Image Image;

        private int _currentFrameIndex;
        private ItemInstance _item;
        private List<Sprite> _spriteCache;

        #endregion Fields

        #region Properties

        public ItemInstance ItemInstance
        {
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

        #endregion Properties

        #region Methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update()
        {
            if (_item != null)
            {
                ReloadSprite();
            }
        }

        private List<Sprite> GetSprites()
        {
            var sprites = new List<Sprite>();
            var tm = Locator.Get<TextureManager>();
            if (ItemInstance.Item != null && ItemInstance.Item.Animations != null)
            {
                for (int i = 0; i < ItemInstance.Item.Animations.Length; i++)
                {
                    var sprite = tm.Items.GetSprite(ItemInstance.Item.Animations[i]);
                    sprites.Add(sprite);
                }
            }

            // :(
            if (sprites.Count() == 0)
            {
                var sprite = tm.Items.GetSprite(1);
                sprites.Add(sprite);
            }

            return sprites;
        }

        private void ReloadSprite()
        {
            if (_spriteCache == null || _spriteCache.Count() == 0)
            {
                return;
            }

            int frameIndex = (int)((Time.time * 6) % _spriteCache.Count()); //smell :(
            if (frameIndex != _currentFrameIndex)
            {
                _currentFrameIndex = frameIndex;
                SetValue(() => Image.Sprite, _spriteCache[frameIndex]);
            }
        }

        #endregion Methods
    }
}