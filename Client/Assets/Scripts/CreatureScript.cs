using UnityEngine;

namespace Gridia
{
    public class CreatureScript : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public Creature Creature { get; set; }

        public void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            if (Creature == null) return;

            if (_spriteRenderer.sprite == null)
            {
                SetupSprite();
            }
//            if (_spriteRenderer.sprite == null)
//            {
//                _spriteRenderer.sprite = Sprite.Create(Resources.Load<Texture2D>("texture"), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 1);
//            }
            var playerLoc = Locator.Get<TileMapView>().Focus.Position;
            transform.localPosition = Locator.Get<GridiaDriver>().GetRelativeScreenPositionForCreature(playerLoc, Creature.Position);
            _spriteRenderer.enabled = playerLoc.z == Creature.Position.z;
        }

        private void SetupSprite()
        {
            if (Creature.Image is DefaultCreatureImage)
            {
                var image = (DefaultCreatureImage) Creature.Image;
                _spriteRenderer.sprite = Locator.Get<TextureManager>().Creatures.GetSprite(image.SpriteIndex, image.Width, image.Height);
            }
            else if (Creature.Image is CustomPlayerImage)
            {
                // TODO: implement custom player image
                _spriteRenderer.sprite = Locator.Get<TextureManager>().Creatures.GetSprite(1);
            }
        }
    }
}
