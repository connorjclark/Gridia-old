using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Player _player;
        private Vector2 _delta;
        private Vector2 _deltaRemaining;
        private float _speed;
        private float _cooldownRemaining;
        private float _cooldown;
        
        public PlayerMovementState (Player player, float speed, float cooldown = 0f)
        {
            _player = player;
            _speed = speed;
            _cooldown = _cooldownRemaining = cooldown;
            _delta = new Vector2 ();
            _deltaRemaining = new Vector2 ();
        }
        
        public void Step (StateMachine stateMachine, float dt)
        {
            if (_delta == Vector2.zero) {
                _delta = ProcessInput ();
                _deltaRemaining = new Vector2 (_delta.x, _delta.y); //smell
            }
            else if (_deltaRemaining != Vector2.zero)
            {
                float stepSpeed = IsRunning () ? _speed * 2 : _speed;
                Vector2 stepDelta = _delta * stepSpeed * dt;

                if (Utilities.CompareAbsoluteValues(_deltaRemaining.x, stepDelta.x) == 1)
                    _deltaRemaining.x -= stepDelta.x;
                else
                    _deltaRemaining.x = 0;
                
                if (Utilities.CompareAbsoluteValues(_deltaRemaining.y, stepDelta.y) == 1)
                    _deltaRemaining.y -= stepDelta.y;
                else
                    _deltaRemaining.y = 0;

                _player.Position += stepDelta;

                if (_deltaRemaining == Vector2.zero) {
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
            _player.Position = new Vector2(Mathf.Round(_player.Position.x), Mathf.Round(_player.Position.y));
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
            Vector2 direction = Direction.None;

            if (Input.GetButton("left"))
                direction += Direction.Left;
            if (Input.GetButton ("right"))
                direction += Direction.Right;
            if (Input.GetButton ("down"))
                direction += Direction.Down;
            if (Input.GetButton ("up"))
                direction += Direction.Up;
            
            return direction;
        }
    }
}