using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ItemMovementState : State
    {
        private Vector3 LocationOfItemToMove { get; set; }
        private Vector3 DestinationSelectorDelta { get; set; }
        private GridiaDriver _driver; // :( move to State?
        private GridiaGame _game;

        public ItemMovementState(Vector3 locationOfItemToMove) 
        {
            LocationOfItemToMove = locationOfItemToMove;
            _driver = Locator.Get<GridiaDriver>();
            _game = Locator.Get<GridiaGame>();
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            _game.hideSelector = false;
            if (HasMoveBeenConfirmed()) 
            {
                var destination = _game.GetSelectorCoord(DestinationSelectorDelta);
                if (destination != LocationOfItemToMove) 
                {
                    var sourceIndex = _driver._game.tileMap.ToIndex(LocationOfItemToMove); // :(
                    var destinationIndex = _driver._game.tileMap.ToIndex(destination);
                    Locator.Get<ConnectionToGridiaServerHandler>().MoveItem("world", "world", sourceIndex, destinationIndex);
                }
                _game.hideSelector = true;
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
            var focusPos = _game.view.FocusPosition;
            var tileSize = 32 * _game.view.Scale;
            var selectorPos = focusPos + DestinationSelectorDelta;
            var selectorRelativePosition = _driver.GetRelativeScreenPosition(focusPos, selectorPos);
            var selectorRect = new Rect(selectorRelativePosition.x, selectorRelativePosition.y, tileSize, tileSize);
            _driver.GUIDrawSelector(selectorRect, new Color32(0, 255, 0, 100));
        }

        private void MoveDestinationSelector()
        {
            var arrowKeysUp = _inputManager.Get4DirectionalArrowKeysInputUp();
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
