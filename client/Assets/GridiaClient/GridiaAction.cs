namespace Gridia
{
    using System;

    using UnityEngine;

    public class GridiaAction
    {
        #region Fields

        private bool _canPerformAction, _requireDestination;
        private long _lastAttack, _timeLeft;

        #endregion Fields

        #region Constructors

        public GridiaAction(int id, String description, bool requireDestination, int cooldownTime, Renderable gfx)
        {
            Id = id;
            Description = description;
            _requireDestination = requireDestination;

            Func<float, String> formatTime = time => String.Format("{0:##.#}s", time/1000.0);
            gfx.OnClick = TriggerAction;
            gfx.OnEnterFrame = () =>
            {
                var timeSinceLastAttack = UnixTimeNow() - _lastAttack;
                _canPerformAction = timeSinceLastAttack >= cooldownTime;

                // :( let's do a circular alpha mask instead of this ...
                _timeLeft = cooldownTime - timeSinceLastAttack;
                var frac = (float) (UnixTimeNow() - _lastAttack)/cooldownTime;
                gfx.Alpha = (byte) (255*Math.Min(1.0, frac));
                if (_timeLeft > 0) GUI.Label(gfx.Rect, formatTime(_timeLeft));
            };
            gfx.ToolTip = () => _canPerformAction
                ? String.Format("Press {0} to use: {1}", id + 1, description)
                : formatTime(_timeLeft);
        }

        #endregion Constructors

        #region Properties

        public String Description
        {
            get; private set;
        }

        public int Id
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

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
                if (Locator.Get<GridiaDriver>().SelectedCreature == null)
                {
                    var creatureNear = Locator.Get<GridiaGame>().GetCreaturesNearPlayer(10, 10, 1);
                    if (creatureNear.Count == 1)
                    {
                        Locator.Get<GridiaDriver>().SelectedCreature = creatureNear[0];
                    }
                    else
                    {
                        return;
                    }
                }
                if (Id == 0)
                {
                    Locator.Get<StateMachine>().SetState(new BarState());
                }
                else
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().PerformAction(Id);
                }
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

        #endregion Methods
    }
}