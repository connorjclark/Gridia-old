namespace Gridia
{
    using UnityEngine;

    public class FloorRenderable : SpriteRenderable
    {
        #region Constructors

        public FloorRenderable(Vector2 pos, int floorIndex)
            : base(pos)
        {
            FloorIndex = floorIndex;
        }

        #endregion Constructors

        #region Properties

        public int FloorIndex
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override int GetSpriteIndex()
        {
            return FloorIndex;
        }

        public override Texture GetTexture(int spriteIndex)
        {
            var textures = Locator.Get<TextureManager>(); // :(
            return textures.Floors.GetTextureForSprite(spriteIndex);
        }

        #endregion Methods
    }
}