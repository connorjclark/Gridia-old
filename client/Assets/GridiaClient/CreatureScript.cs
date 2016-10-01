namespace Gridia
{
    using UnityEngine;
    using UnityEngine.UI;

    public class CreatureScript : MonoBehaviour
    {
        #region Fields

        private Creature _creature;
        private GridiaDriver _driver;
        private Text _nameText;
        private SpriteRenderer _spriteRenderer;
        private StatusCircle _statusCircle;

        #endregion Fields

        #region Properties

        public Creature Creature
        {
            get { return _creature; }
            set
            {
                _creature = value;
                _creature.CreatureScript = this;
            }
        }

        #endregion Properties

        #region Methods

        public void ClearImage()
        {
            _spriteRenderer.sprite = null;
        }

        public void OnMouseOver()
        {
            if (Input.GetMouseButton(0) && _driver.SelectedCreature != Creature)
            {
                _driver.SelectedCreature = Creature;
            }
        }

        public void Start()
        {
            _driver = Locator.Get<GridiaDriver>();
            _statusCircle = GetComponent<StatusCircle>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var canvas = Instantiate(Resources.Load("Text")) as GameObject;
            canvas.transform.SetParent(gameObject.transform, false);
            _nameText = canvas.GetComponentInChildren<Text>();
            _nameText.transform.localPosition = new Vector2(0, 32);
            _nameText.fontStyle = FontStyle.Bold;
            _nameText.color = Color.white;
        }

        public void Update()
        {
            if (Creature == null) return;

            if (_spriteRenderer.sprite == null)
            {
                SetupSprite();
                _nameText.text = Creature.Name;
            }
            var playerLoc = Locator.Get<TileMapView>().Focus.Position;
            transform.localPosition = Locator.Get<GridiaDriver>().GetRelativeScreenPositionForCreature(playerLoc, Creature.Position);
            SetVisibility(playerLoc.z == Creature.Position.z);
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

        // :(
        private void SetVisibility(bool v)
        {
            _spriteRenderer.enabled = v;
            _nameText.enabled = v;
            var isSelected = _driver.SelectedCreature == Creature;
            var isPlayer = _driver.Game.View.Focus == Creature;
            _statusCircle.enabled = v && (isSelected || isPlayer);
            _statusCircle.text.enabled = v && (isSelected || isPlayer);
            GetComponent<LineRenderer>().enabled = v;
        }

        #endregion Methods
    }
}