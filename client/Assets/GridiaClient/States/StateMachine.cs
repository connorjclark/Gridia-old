namespace Gridia
{
    public abstract class State
    {
        #region Fields

        protected InputManager InputManager = Locator.Get<InputManager>();

        #endregion Fields

        #region Methods

        public virtual void Enter(StateMachine stateMachine)
        {
        }

        public virtual void OnGUI()
        {
        }

        public abstract void Step(StateMachine stateMachine, float dt);

        #endregion Methods
    }

    public class StateMachine
    {
        #region Properties

        public State CurrentState
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void OnGUI()
        {
            if (CurrentState != null)
            {
                CurrentState.OnGUI();
            }
        }

        public void SetState(State state)
        {
            CurrentState = state;
            CurrentState.Enter(this);
        }

        public void Step(float dt)
        {
            if (CurrentState != null)
            {
                CurrentState.Step(this, dt);
            }
        }

        #endregion Methods
    }
}