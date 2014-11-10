using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private float _speed;
        private long _cooldownUntil;
        
        public PlayerMovementState (float speed)
        {
            _speed = speed;
        }

        // : (
        private long getSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - GridiaConstants.SERVER_TIME_OFFSET;
        }
        
        public void Step (StateMachine stateMachine, float dt)
        {
            if (Player == null) return;
            if (_cooldownUntil == 0)
            {
                var delta = ProcessInput ();
                if (delta != Vector3.zero) 
                {
                    var pos = Player.Position;
                    var newPosition = pos + delta;
                    var now = getSystemTime();
                    _cooldownUntil = now + 100;
                    Player.AddPositionSnapshot(pos, now - Creature.RENDER_DELAY);
                    Player.AddPositionSnapshot(newPosition, _cooldownUntil - Creature.RENDER_DELAY);
                }
            }
            else if (getSystemTime() > _cooldownUntil)
            {
                End(stateMachine, dt);
            }
        }

        private bool IsRunning ()
        {
            return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
        }

        private void End (StateMachine stateMachine, float dt)
        {
            stateMachine.CurrentState = new PlayerMovementState(_speed);
            stateMachine.Step (dt);
        }

        private Vector3 ProcessInput ()
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