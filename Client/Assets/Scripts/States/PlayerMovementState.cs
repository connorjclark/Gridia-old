using System;
using UnityEngine;

namespace Gridia
{
    public class PlayerMovementState : State
    {
        private static Creature Player { get { return Locator.Get<TileMapView>().Focus; } }// :(
        private long _cooldownUntil;
        private readonly Vector3 _delta;

        public PlayerMovementState(Vector3 delta)
        {
            _delta = delta;
        }

        // :(
        private long GetSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - GridiaConstants.ServerTimeOffset;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            if (Player == null) return;
            if (_cooldownUntil == 0)
            {
                Locator.Get<GridiaDriver>().SelectedCreature = null;
                var destination = Locator.Get<GridiaGame>().View.Focus.Position + _delta;

                var now = GetSystemTime();

                const int baseTime = 250; // :(
                var floor = Locator.Get<GridiaGame>().TileMap.GetTile((int)destination.x, (int)destination.y, (int)destination.z).Floor;
                var movementModifier = Locator.Get<ContentManager>().GetFloor(floor).MovementModifier;
                var timeForMovement = baseTime / movementModifier;
                var onRaft = false;

                if (floor == 1 && Locator.Get<GridiaDriver>().InvGui.HasRaft())
                {
                    timeForMovement = baseTime / 2;
                    onRaft = true;
                }

                Locator.Get<ConnectionToGridiaServerHandler>().PlayerMove(_delta, onRaft, (int)timeForMovement);

                _cooldownUntil = now + (int)timeForMovement;
                Player.AddPositionSnapshot(Player.Position, onRaft, now - Creature.RENDER_DELAY);
                Player.AddPositionSnapshot(destination, onRaft, _cooldownUntil - Creature.RENDER_DELAY);
            }
            else if (GetSystemTime() > _cooldownUntil)
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
