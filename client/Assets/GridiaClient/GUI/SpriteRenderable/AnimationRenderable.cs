namespace Gridia
{
    using UnityEngine;

    public class AnimationRenderable : SpriteRenderable
    {
        #region Constructors

        public AnimationRenderable(Vector3 coord, GridiaAnimation animation, bool loop = false, bool isInWorld = true)
            : base(Vector2.zero)
        {
            RenderBox = false;
            Coord = coord;
            Animation = animation;
            OnNewFrame = true;
            Loop = loop;
            IsInWorld = isInWorld;
        }

        #endregion Constructors

        #region Properties

        public bool Dead
        {
            get; private set;
        }

        public bool IsInWorld
        {
            get; set;
        }

        public bool Loop
        {
            get; set;
        }

        private GridiaAnimation Animation
        {
            get; set;
        }

        private Vector3 Coord
        {
            get; set;
        }

        private Frame CurrentFrame
        {
            get { return Animation.Frames[CurrentFrameIndex]; }
        }

        private int CurrentFrameIndex
        {
            get; set;
        }

        private bool OnNewFrame
        {
            get; set;
        }

        private float TimeElapsed
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override int GetSpriteIndex()
        {
            return CurrentFrame.Sprite;
        }

        public override Texture GetTexture(int spriteIndex)
        {
            var textures = Locator.Get<TextureManager>(); // :(
            return textures.Animations.GetTextureForSprite(spriteIndex);
        }

        public void Step(float dt)
        {
            if (IsInWorld)
            {
                var screenPos = Locator.Get<GridiaGame>().GetScreenPosition(Coord);
                X = screenPos.x;
                Y = screenPos.y;
            }

            TimeElapsed += dt;
            var newFrameIndex = (int) (TimeElapsed/0.25);
            if (newFrameIndex < Animation.Frames.Count || Loop)
            {
                if (OnNewFrame)
                {
                    OnNewFrame = false;
                    CurrentFrameIndex = newFrameIndex%Animation.Frames.Count;
                    if (CurrentFrame.Sound != null)
                    {
                        Locator.Get<SoundPlayer>().PlaySfxAt(CurrentFrame.Sound, Coord);
                    }
                }
                else
                {
                    OnNewFrame = newFrameIndex != CurrentFrameIndex;
                }
            }
            else
            {
                Dead = true;
            }
        }

        #endregion Methods
    }
}