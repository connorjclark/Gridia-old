using UnityEngine;

namespace Gridia
{
    public class BarState : State
    {
        private readonly GridiaAction _action;
        private GameObject go;
        private bool begun = false;

        public override void Enter(StateMachine stateMachine)
        {
            MainThreadQueue.Add(() =>
            {
                go = GameObject.Instantiate(Resources.Load("Bar")) as GameObject;
                begun = true;
            });
            Locator.Get<GridiaDriver>().TabbedGui.Visible = false;
        }

        public override void Step(StateMachine stateMachine, float dt)
        {
            if (begun && go == null)
            {
                Locator.Get<GridiaDriver>().TabbedGui.Visible = true;
                Locator.Get<ConnectionToGridiaServerHandler>().SetDefense(0);
                stateMachine.SetState(new IdleState());
            }
        }
    }
}
