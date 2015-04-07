using System;
using UnityEngine;

namespace Gridia
{
    public class ActionWindow : GridiaWindow
    {
        protected ExtendibleGrid ActionGrid = new ExtendibleGrid(Vector2.zero); // :(

        public ActionWindow(Vector2 pos)
            : base(pos, "Actions")
        {
            Resizeable = false;
            AddChild(ActionGrid);
        }

        public void TempAddActions()
        {
            TempAddAction(1, 1000, "Attack with your equipped weapon.", "Attack", false);
            TempAddAction(2, 3000, "Roll quickly to a nearby tile.", "Blade", true);
        }

        public void AddAction(int id, int cooldownTime, String description, Renderable gfx, Boolean requireDestination)
        {
            gfx.ScaleXY = 2;

            var lastAttack = UnixTimeNow();
            var canPerformAction = true;
            var timeLeft = 0L;

            gfx.OnClick = () =>
            {
                if (canPerformAction/* && Locator.Get<GridiaDriver>().SelectedCreature != null*/)
                {
                    if (requireDestination)
                    {
                        // go into a destination picker state ...
                    }
                    else
                    {
                        Locator.Get<ConnectionToGridiaServerHandler>().PerformAction(id);
                        lastAttack = UnixTimeNow();
                    }
                }
            };
            
            gfx.OnEnterFrame = () =>
            {
                var timeSinceLastAttack = UnixTimeNow() - lastAttack;
                canPerformAction = timeSinceLastAttack >= cooldownTime;

                // :( let's do a circular alpha mask instead of this ...
                timeLeft = cooldownTime - timeSinceLastAttack;
                var frac = (float) (UnixTimeNow() - lastAttack)/cooldownTime;
                gfx.Alpha = (byte)(255 * Math.Min(1.0, frac));
            };

            gfx.ToolTip = () => canPerformAction ? description : String.Format("{0:##.#}s", timeLeft/1000.0);

            ActionGrid.AddChild(gfx);
        }

        // in ms
        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }

        private void TempAddAction(int id, int cooldownTime, String description, String animName, Boolean requireDestination)
        {
            var cm = Locator.Get<ContentManager>();
            var anim = cm.GetAnimation(animName);
            var renderable = new AnimationRenderable(Vector2.zero, anim);
            AddAction(id, cooldownTime, description, renderable, requireDestination);
        }
    }
}
