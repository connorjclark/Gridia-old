namespace Gridia
{
    public abstract class State
    {
        public abstract void Step(StateMachine stateMachine, float dt);
        public virtual void OnGUI() { }
        protected InputManager InputManager = Locator.Get<InputManager>();
    }

    public class StateMachine
    {
        public State CurrentState { get; private set; }
        
        public void SetState(State state)
        {
             CurrentState = state;
        }
        
        public void Step (float dt)
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