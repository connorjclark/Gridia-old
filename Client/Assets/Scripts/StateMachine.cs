namespace Gridia
{
    public interface State
    {
        void Step (StateMachine stateMachine, float dt);
    }

    public class StateMachine
    {

        public State CurrentState { get; set; }
        
        public void SetState (State state)
        {
            CurrentState = state;
        }
        
        public void Step (float dt)
        {
            if (CurrentState != null)
                CurrentState.Step (this, dt);
        }
    }
}