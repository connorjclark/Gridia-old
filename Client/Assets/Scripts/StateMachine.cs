using UnityEngine;
namespace Gridia
{
    public abstract class State
    {
        public abstract void Step (StateMachine stateMachine, float dt);
        protected InputManager _inputManager = Locator.Get<InputManager>();
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
    }
}