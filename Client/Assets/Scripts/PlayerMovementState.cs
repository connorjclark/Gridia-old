using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private long _cooldownUntil;

        // : (
        private long getSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - GridiaConstants.SERVER_TIME_OFFSET;
        }

        public override void Step(StateMachine stateMachine, float dt)
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
                else
                {
                    End(stateMachine);
                }
            }
            else if (getSystemTime() > _cooldownUntil)
            {
                End(stateMachine);
            }
        }

        private bool IsRunning ()
        {
            return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
        }

        private void End (StateMachine stateMachine)
        {
            stateMachine.CurrentState = new IdleState();
        }

        private Vector3 ProcessInput ()
        {
            Vector3 direction = ProcessDirectionalInput();

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