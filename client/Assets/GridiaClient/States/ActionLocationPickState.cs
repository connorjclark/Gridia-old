// :( this is a lot like ItemMovementState
namespace Gridia
{
    using UnityEngine;

    public class ActionLocationPickState : State
    {
        #region Fields

        private readonly GridiaAction _action;
        private readonly GridiaDriver _driver; // :( move to State?
        private readonly GridiaGame _game;

        private Vector3 _destinationSelectorDelta = Vector3.zero; // :(

        #endregion Fields

        #region Constructors

        public ActionLocationPickState(GridiaAction action)
        {
            _driver = Locator.Get<GridiaDriver>();
            _game = Locator.Get<GridiaGame>();
            _action = action;
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
                _destinationSelectorDelta.x = Mathf.Clamp(_destinationSelectorDelta.x, -10, 10);
                _destinationSelectorDelta.y = Mathf.Clamp(_destinationSelectorDelta.y, -10, 10);
            }
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
            _game.HideSelector = true;
            if (HasMoveBeenConfirmed())
            {
                var destination = _game.GetSelectorCoord(DestinationSelectorDelta);
                _action.TriggerAction(destination);
                stateMachine.SetState(new IdleState());
            }
            else
            {
                MoveDestinationSelector();
            }
        }

        private bool HasMoveBeenConfirmed()
        {
            return Input.GetKeyUp(KeyCode.Space);
        }

        private void MoveDestinationSelector()
        {
            var delta = InputManager.Get4DirectionalArrowKeysInputUp();
            if (delta == Vector3.zero)
            {
                delta = InputManager.Get4DirectionalWasdInputUp();
            }
            if (delta != Vector3.zero)
            {
                DestinationSelectorDelta += delta;
            }
        }

        #endregion Methods
    }
}