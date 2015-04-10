namespace Gridia
{
    public abstract class State
    {
        public virtual void Enter(StateMachine stateMachine) {}
        public abstract void Step(StateMachine stateMachine, float dt);
        public virtual void OnGUI() {}
        protected InputManager InputManager = Locator.Get<InputManager>();
    }

    public class StateMachine
    {
        public State CurrentState { get; private set; }

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

        public void OnGUI()
        {
            if (CurrentState != null)
            {
                CurrentState.OnGUI();
            }
        }
    }
}