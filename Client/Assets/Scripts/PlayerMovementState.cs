using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private long _cooldownUntil;
        private Vector3 _destination;

        public PlayerMovementState(Vector3 destination)
        {
            _destination = destination;
        }

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
                Locator.Get<ConnectionToGridiaServerHandler>().PlayerMove(_destination);
                var now = getSystemTime();

                // :(
                var baseTime = 150;
                float movementModifier;
                var floor = Locator.Get<GridiaGame>().tileMap.GetTile((int)_destination.x, (int)_destination.y, (int)_destination.z).Floor;
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
                Player.AddPositionSnapshot(Player.Position, now - Creature.RENDER_DELAY);
                Player.AddPositionSnapshot(_destination, _cooldownUntil - Creature.RENDER_DELAY);
            }
            else if (getSystemTime() > _cooldownUntil)
            {
                End(stateMachine);
            }
        }

        private void End(StateMachine stateMachine)
        {
            // :(
            stateMachine.SetState(new IdleState());
        }
    }
}