﻿using System;
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

        public ItemUseState(String source, int sourceIndex) 
        {
            _source = source;
            _sourceIndex = sourceIndex;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            var endTheState = Input.GetKeyUp(KeyCode.Space);

            var direction = ProcessDirectionalInput();
            if (direction != Vector3.zero || endTheState) 
            {
                var destLocation = Locator.Get<TileMapView>().Focus.Position + direction;
                var destIndex = Locator.Get<TileMap>().ToIndex(destLocation);
                Locator.Get<ConnectionToGridiaServerHandler>().UseItem(_source, "world", _sourceIndex, destIndex);
                endTheState = true;
            }

            if (endTheState)
            {
                End(stateMachine);
            }
        }

        private void End(StateMachine stateMachine)
        {
            stateMachine.SetState(new IdleState());
        }
    }
}