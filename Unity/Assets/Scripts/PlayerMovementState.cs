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
            if (IsZero (_delta)) {
                _delta = ProcessInput ();
                _deltaRemaining = new Vector2 (_delta.x, _delta.y); //smell
            } else if (!IsZero (_deltaRemaining)) {
                float stepSpeed = IsRunning () ? _speed * 2 : _speed;
                Vector2 stepDelta = _delta * stepSpeed * dt;
                
                if (Mathf.Abs (_deltaRemaining.x) > Mathf.Abs (stepDelta.x))
                    _deltaRemaining.x -= stepDelta.x;
                else
                    _deltaRemaining.x = 0;
                
                if (Mathf.Abs (_deltaRemaining.y) > Mathf.Abs (stepDelta.y))
                    _deltaRemaining.y -= stepDelta.y;
                else
                    _deltaRemaining.y = 0;


                _player.Position += stepDelta;

                if (IsZero(_deltaRemaining)) {
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

        private bool IsZero (Vector2 vector)
        {
            return vector.x == 0 && vector.y == 0;
        }
        
        private Vector2 ProcessInput ()
        {
            int dx = 0;
            int dy = 0;
            
            if (Input.GetButton ("left"))
                dx = -1;
            if (Input.GetButton ("right"))
                dx = 1;
            if (Input.GetButton ("down"))
                dy = -1;
            if (Input.GetButton ("up"))
                dy = 1;
            
            return new Vector2 (dx, dy);
        }
    }
}