using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private long _cooldownUntil;

        // :(
        private long getSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - GridiaConstants.SERVER_TIME_OFFSET;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            if (Player == null) return;
            if (_cooldownUntil == 0)
            {
                var delta = ProcessInput();
                if (delta != Vector3.zero) 
                {
                    var pos = Player.Position;
                    var newPosition = pos + delta;
                    var now = getSystemTime();
                    var baseTime = 200;
                    
                    // :(
                    float movementModifier;
                    var destination = Locator.Get<GridiaGame>().view.Focus.Position + delta;
                    var floor = Locator.Get<GridiaGame>().tileMap.GetTile((int)destination.x, (int)destination.y, (int)destination.z).Floor;
                    switch (floor) 
                    {
                        case 1:
                            movementModifier = 0.3f;
                            break;
                        case 21:
                        case 47:
                            movementModifier = 0.8f;
                            break;
                        case 2:
                        case 11:
                        case 18:
                        case 46:
                            movementModifier = 1.5f;
                            break;
                        default:
                            movementModifier = 1f;
                            break;
                    }
                    
                    var timeForMovement = baseTime / movementModifier;
                    _cooldownUntil = now + (int)timeForMovement;
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

        private bool IsRunning()
        {
            return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
        }

        private void End(StateMachine stateMachine)
        {
            stateMachine.SetState(new IdleState());
        }

        private Vector3 ProcessInput()
        {
            var direction = _inputManager.Get4DirectionalInput();

            if (direction != Vector3.zero)
            {
                Vector3 destination = Locator.Get<GridiaGame>().view.Focus.Position + direction;
                if (!Locator.Get<GridiaGame>().tileMap.Walkable((int)destination.x, (int)destination.y, (int)destination.z))
                {
                    direction = Vector3.zero;
                }
                else 
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().PlayerMove(destination);
                }
            }
            
            return direction;
        }
    }
}