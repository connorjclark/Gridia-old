using UnityEngine;

namespace Gridia
{
    public class ItemRenderable : SpriteRenderable
    {
        public ItemInstance Item { get; set; }

        public ItemRenderable(Vector2 pos, ItemInstance item)
            : base(pos)
        {
            Item = item;
            ToolTip = () => Item.Item.Id != 0 ? Item.ToString() : null;
        }

        public override int GetSpriteIndex()
        {
            var animations = Item.Item.Animations;
            if (animations == null)
            {
                return 1;
            }
            var frame = (int)((Time.time * 6) % animations.Length); //smell :(
            return animations[frame];
        }

        public override Texture GetTexture(int spriteIndex)
        {
            var textures = Locator.Get<TextureManager>(); // :(
            return textures.GetItemsTexture(spriteIndex / GridiaConstants.SPRITES_IN_SHEET);
        }
    }
}
