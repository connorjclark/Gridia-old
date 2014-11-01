using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature _player;
        private Vector3 _delta;
        private Vector3 _deltaRemaining;
        private float _speed;
        private float _cooldownRemaining;
        private float _cooldown;
        
        public PlayerMovementState (Creature player, float speed, float cooldown = 0f)
        {
            _player = player;
            _speed = speed;
            _cooldown = _cooldownRemaining = cooldown;
            _delta = new Vector3 ();
            _deltaRemaining = new Vector3 ();
        }
        
        public void Step (StateMachine stateMachine, float dt)
        {
            if (_delta == Vector3.zero) {
                _delta = ProcessInput ();
                _deltaRemaining = new Vector3 (_delta.x, _delta.y); //smell
            }
            else if (_deltaRemaining != Vector3.zero)
            {
                float stepSpeed = IsRunning () ? _speed * 2 : _speed;
                Vector3 stepDelta = _delta * stepSpeed * dt;

                if (Utilities.CompareAbsoluteValues(_deltaRemaining.x, stepDelta.x) == 1)
                    _deltaRemaining.x -= stepDelta.x;
                else
                    _deltaRemaining.x = 0;
                
                if (Utilities.CompareAbsoluteValues(_deltaRemaining.y, stepDelta.y) == 1)
                    _deltaRemaining.y -= stepDelta.y;
                else
                    _deltaRemaining.y = 0;

                _player.Offset += stepDelta;

                if (_deltaRemaining == Vector3.zero) {
                    StartCooldown(stateMachine, dt);
                }
            } else {
                StartCooldown(stateMachine, dt);
            }
        }

        private bool IsRunning ()
        {
            return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
        }

        private void StartCooldown(StateMachine stateMachine, float dt)
        {
            _player.Offset = Vector2.zero;
            Locator.Get<GridiaGame>().tileMap.UpdateCreature(_player, _player.Position + _delta);
            int player_x = (int)Mathf.Round(_player.Position.x);
            int player_y = (int)Mathf.Round(_player.Position.y);
            //stateMachine.ServerConnection.MovePlayer(player_x, player_y);
            Cooldown(stateMachine, dt);
        }

        private void Cooldown (StateMachine stateMachine, float dt)
        {
            _cooldownRemaining -= dt;
            if (_cooldownRemaining <= 0) {
                End (stateMachine, dt);
            }
        }

        private void End (StateMachine stateMachine, float dt)
        {
            stateMachine.CurrentState = new PlayerMovementState (_player, _speed, _cooldown);
            stateMachine.Step (dt);
        }

        private Vector2 ProcessInput ()
        {
            Vector3 direction = Direction.None;

            if (Input.GetButton("left"))
                direction += Direction.Left;
            if (Input.GetButton ("right"))
                direction += Direction.Right;
            if (Input.GetButton ("down"))
                direction += Direction.Down;
            if (Input.GetButton ("up"))
                direction += Direction.Up;

            if (direction != Direction.None)
            {
                Vector3 destination = Locator.Get<GridiaGame>().view.Focus.Position + direction;
                if (!Locator.Get<GridiaGame>().tileMap.Walkable((int)destination.x, (int)destination.y, (int)destination.z))
                {
                    direction = Direction.None;
                }
            }
            
            return direction;
        }
    }
}