using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private Creature Player { get { return Locator.Get<TileMapView>().Focus; } }
        private long _cooldownUntil;
        private Vector3 _delta;

        public PlayerMovementState(Vector3 delta)
        {
            _delta = delta;
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
                var destination = Locator.Get<GridiaGame>().view.Focus.Position + _delta;

                var now = getSystemTime();

                var baseTime = 250; // :(
                var floor = Locator.Get<GridiaGame>().tileMap.GetTile((int)destination.x, (int)destination.y, (int)destination.z).Floor;
                float movementModifier = Locator.Get<ContentManager>().GetFloor(floor).MovementModifier;
                var timeForMovement = baseTime / movementModifier;

                Locator.Get<ConnectionToGridiaServerHandler>().PlayerMove(_delta, (int)timeForMovement);

                _cooldownUntil = now + (int)timeForMovement;
                Player.AddPositionSnapshot(Player.Position, now - Creature.RENDER_DELAY);
                Player.AddPositionSnapshot(destination, _cooldownUntil - Creature.RENDER_DELAY);
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