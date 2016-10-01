using UnityEngine;

namespace Gridia
{
    public class ItemMovementState : State
    {
        private Vector3 LocationOfItemToMove { get; set; }
        private Vector3 _destinationSelectorDelta = Vector3.zero; // :(
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
        private readonly GridiaDriver _driver; // :( move to State?
        private readonly GridiaGame _game;

        public ItemMovementState(Vector3 locationOfItemToMove) 
        {
            LocationOfItemToMove = locationOfItemToMove;
            _driver = Locator.Get<GridiaDriver>();
            _game = Locator.Get<GridiaGame>();
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

        private void MoveDestinationSelector()
        {
            var arrowKeysUp = InputManager.Get4DirectionalArrowKeysInputUp();
            if (arrowKeysUp != Vector3.zero)
            {
                DestinationSelectorDelta += arrowKeysUp;
            }
        }

        private bool HasMoveBeenConfirmed() 
        {
            return !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt);
        }
    }
}
