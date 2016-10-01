namespace Gridia
{
    using UnityEngine;

    public class BarState : State
    {
        #region Fields

        private readonly GridiaAction _action;

        private bool begun = false;
        private GameObject go;

        #endregion Fields

        #region Methods

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

        #endregion Methods
    }
}