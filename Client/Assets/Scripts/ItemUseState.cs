using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ItemUseState : State
    {
        private String _source;
        private int _sourceIndex;
        private Vector3 _previousDirection = new Vector3(123, 123);

        public ItemUseState(String source, int sourceIndex) 
        {
            _source = source;
            _sourceIndex = sourceIndex;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.LeftAlt)) 
            {
                End(stateMachine);
            }
            if (_inputManager.Valid9DirectionalInputUp())
            {
                var direction = _inputManager.Get9DirectionalInputUp();
                if (direction != _previousDirection)
                {
                    _previousDirection = direction;
                    var destLocation = Locator.Get<TileMapView>().Focus.Position + direction;
                    var destIndex = Locator.Get<TileMap>().ToIndex(destLocation);
                    Locator.Get<ConnectionToGridiaServerHandler>().UseItem(_source, "world", _sourceIndex, destIndex);
                }
            }
        }

        private void End(StateMachine stateMachine)
        {
            stateMachine.SetState(new IdleState());
        }
    }
}
