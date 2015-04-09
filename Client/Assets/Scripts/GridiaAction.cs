using System;
using UnityEngine;

namespace Gridia
{
    public class GridiaAction
    {
        public int Id { get; private set; }
        public String Description { get; private set; }
        private long _lastAttack, _timeLeft, _cooldownTime;
        private bool _canPerformAction, _requireDestination;

        public GridiaAction(int id, String description, bool requireDestination, int cooldownTime, Renderable gfx)
        {
            Id = id;
            Description = description;
            _requireDestination = requireDestination;
            _cooldownTime = cooldownTime;
            _lastAttack = UnixTimeNow();

            gfx.OnClick = TriggerAction;
            gfx.OnEnterFrame = () =>
            {
                var timeSinceLastAttack = UnixTimeNow() - _lastAttack;
                _canPerformAction = timeSinceLastAttack >= cooldownTime;

                // :( let's do a circular alpha mask instead of this ...
                _timeLeft = cooldownTime - timeSinceLastAttack;
                var frac = (float)(UnixTimeNow() - _lastAttack) / cooldownTime;
                gfx.Alpha = (byte)(255 * Math.Min(1.0, frac));
            };
            gfx.ToolTip = () => _canPerformAction ? description : String.Format("{0:##.#}s", _timeLeft / 1000.0);
        }

        public void TriggerAction()
        {
            if (!_canPerformAction) return;
            if (_requireDestination)
            {
                var pickState = new ActionLocationPickState(this);
                Locator.Get<StateMachine>().SetState(pickState);
            }
            else
            {
                Locator.Get<ConnectionToGridiaServerHandler>().PerformAction(Id);
                _lastAttack = UnixTimeNow();
            }
        }

        public void TriggerAction(Vector3 destination)
        {
            if (!_canPerformAction) return;
            Locator.Get<ConnectionToGridiaServerHandler>().PerformAction(Id, destination);
            _lastAttack = UnixTimeNow();
        }

        // in ms
        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }
    }
}
