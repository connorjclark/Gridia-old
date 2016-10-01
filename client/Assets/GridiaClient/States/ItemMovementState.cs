namespace Gridia
{
    using UnityEngine;

    public class ItemMovementState : State
    {
        #region Fields

        private readonly GridiaDriver _driver; // :( move to State?
        private readonly GridiaGame _game;

        private Vector3 _destinationSelectorDelta = Vector3.zero; // :(

        #endregion Fields

        #region Constructors

        public ItemMovementState(Vector3 locationOfItemToMove)
        {
            LocationOfItemToMove = locationOfItemToMove;
            _driver = Locator.Get<GridiaDriver>();
            _game = Locator.Get<GridiaGame>();
        }

        #endregion Constructors

        #region Properties

        public Vector3 DestinationSelectorDelta
        {
            get
            {
                return _destinationSelectorDelta;
            }
            set
            {
                _destinationSelectorDelta = value;
                _destinationSelectorDelta.x = Mathf.Clamp(_destinationSelectorDelta.x, -2, 2);
                _destinationSelectorDelta.y = Mathf.Clamp(_destinationSelectorDelta.y, -2, 2);
            }
        }

        private Vector3 LocationOfItemToMove
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void OnGUI()
        {
            base.OnGUI();
            var focusPos = _game.View.FocusPosition;
            var tileSize = 32 * _game.View.Scale;
            var selectorPos = focusPos + DestinationSelectorDelta;
            var selectorRelativePosition = _driver.GetRelativeScreenPosition(focusPos, selectorPos);
            var selectorRect = new Rect(selectorRelativePosition.x, selectorRelativePosition.y, tileSize, tileSize);
            GridiaConstants.GUIDrawSelector(selectorRect, new Color32(0, 255, 0, 100));
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            _game.HideSelector = false;
            if (HasMoveBeenConfirmed())
            {
                var destination = _game.GetSelectorCoord(DestinationSelectorDelta);
                if (destination != LocationOfItemToMove)
                {
                    var sourceIndex = _driver.Game.TileMap.ToIndex(LocationOfItemToMove); // :(
                    var destinationIndex = _driver.Game.TileMap.ToIndex(destination);
                    Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(0, 0, sourceIndex, destinationIndex);
                }
                _game.HideSelector = true;
                stateMachine.SetState(new IdleState());
            }
            else
            {
                MoveDestinationSelector();
            }
        }

        private bool HasMoveBeenConfirmed()
        {
            return !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt);
        }

        private void MoveDestinationSelector()
        {
            var arrowKeysUp = InputManager.Get4DirectionalArrowKeysInputUp();
            if (arrowKeysUp != Vector3.zero)
            {
                DestinationSelectorDelta += arrowKeysUp;
            }
        }

        #endregion Methods
    }
}