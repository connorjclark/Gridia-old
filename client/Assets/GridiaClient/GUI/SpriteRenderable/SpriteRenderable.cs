namespace Gridia
{
    using UnityEngine;

    public abstract class SpriteRenderable : Renderable
    {
        #region Constructors

        public SpriteRenderable(Vector2 pos)
            : base(pos)
        {
            RenderBox = true;
            _rect.width = _rect.height = GridiaConstants.SpriteSize;
        }

        #endregion Constructors

        #region Properties

        protected bool RenderBox
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public abstract int GetSpriteIndex();

        public abstract Texture GetTexture(int spriteIndex);

        public override void Render()
        {
            base.Render();

            if (RenderBox)
            {
                GUI.Box(Rect, "");
            }

            var textures = Locator.Get<TextureManager>(); // :(
            var spriteIndex = GetSpriteIndex();
            var texture = GetTexture(spriteIndex);

            var textureX = (spriteIndex % GridiaConstants.SpritesInSheet) % GridiaConstants.NumTilesInSpritesheetRow;
            var textureY = 9 - (spriteIndex % GridiaConstants.SpritesInSheet) / GridiaConstants.NumTilesInSpritesheetRow;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(Rect, texture, texCoords);
        }

        #endregion Methods
    }
}