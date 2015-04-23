using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class ActionWindow : GridiaWindow
    {
        protected ExtendibleGrid ActionGrid = new ExtendibleGrid(Vector2.zero); // :(
        private Dictionary<int, GridiaAction> _actions = new Dictionary<int, GridiaAction>(); 

        public ActionWindow(Vector2 pos)
            : base(pos, "Actions")
        {
            Resizeable = false;
            AddChild(ActionGrid);
        }

        public void TriggerAction(int id)
        {
            if (_actions.ContainsKey(id))
            {
                _actions[id].TriggerAction();
            }
        }

        public void TempAddActions()
        {
            TempAddAction(0, "Attack with your equipped weapon.", false, 1000, "Attack");
            TempAddAction(1, "Dash quickly to a nearby tile. Use WASD/Arrows and press Space to select a destination.", true, 3000, "Blade");
            TempAddAction(2, "Cast fire spell.", false, 5000, "Flame");
            TempAddAction(3, "Cast a healing spell.", false, 15000, "Heal");
        }

        public void AddAction(int id, string description, bool requireDestination, int cooldownTime, Renderable gfx)
        {
            var action = new GridiaAction(id, description, requireDestination, cooldownTime, gfx);
            _actions.Add(id, action);
            gfx.ScaleXY = 2;
            ActionGrid.AddChild(gfx);
        }

        private void TempAddAction(int id, string description, bool requireDestination, int cooldownTime, string animName)
        {
            var cm = Locator.Get<ContentManager>();
            var anim = cm.GetAnimation(animName);
            var renderable = new AnimationRenderable(Vector2.zero, anim, true, false);
            Locator.Get<GridiaGame>().Animations.Add(renderable); // :(
            AddAction(id, description, requireDestination, cooldownTime, renderable);
        }
    }
}
