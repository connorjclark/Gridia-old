using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private Vector3 _delta;
        private Vector3 _deltaRemaining;
        private float _speed;
        private float _cooldownRemaining;
        private float _cooldown;
        
        public PlayerMovementState (float speed, float cooldown = 0f)
        {
            _speed = speed;
            _cooldown = _cooldownRemaining = cooldown;
            _delta = new Vector3 ();
            _deltaRemaining = new Vector3 ();
        }
        
        public void Step (StateMachine stateMachine, float dt)
        {
            if (Player == null) return;
            if (_delta == Vector3.zero) {
                _delta = ProcessInput ();
                _deltaRemaining = new Vector3 (_delta.x, _delta.y, 0); //smell
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

                Player.Offset += stepDelta;

                if (_deltaRemaining == Vector3.zero) {
                    StartCooldown(stateMachine, dt);
                }
            } else {
                Cooldown(stateMachine, dt);
            }
        }

        private bool IsRunning ()
        {
            return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
        }

        private void StartCooldown(StateMachine stateMachine, float dt)
        {
            Creature player = Player;
            player.Offset = Vector3.zero;
            Locator.Get<GridiaGame>().tileMap.UpdateCreature(player, player.Position + _delta);
            //int player_x = (int)Mathf.Round(_player.Position.x);
            //int player_y = (int)Mathf.Round(_player.Position.y);
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
            stateMachine.CurrentState = new PlayerMovementState(_speed, _cooldown);
            stateMachine.Step (dt);
        }

        private Vector2 ProcessInput ()
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetButton("left"))
                direction += Vector3.left;
            if (Input.GetButton ("right"))
                direction += Vector3.right;
            if (Input.GetButton ("down"))
                direction += Vector3.down;
            if (Input.GetButton ("up"))
                direction += Vector3.up;

            if (direction != Vector3.zero)
            {
                Vector3 destination = Locator.Get<GridiaGame>().view.Focus.Position + direction;
                if (!Locator.Get<GridiaGame>().tileMap.Walkable((int)destination.x, (int)destination.y, (int)destination.z))
                {
                    direction = Vector3.zero;
                }
                else {
                    Locator.Get<ConnectionToGridiaServerHandler>().PlayerMove(destination);
                }
            }
            
            return direction;
        }
    }
}