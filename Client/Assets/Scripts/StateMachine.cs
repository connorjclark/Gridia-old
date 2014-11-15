using UnityEngine;
namespace Gridia
{
    public abstract class State
    {
        public abstract void Step (StateMachine stateMachine, float dt);

        protected Vector3 ProcessDirectionalInput() 
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetButton("left"))
                direction += Vector3.left;
            if (Input.GetButton("right"))
                direction += Vector3.right;
            if (Input.GetButton("down"))
                direction += Vector3.down;
            if (Input.GetButton("up"))
                direction += Vector3.up;

            return direction;
        }
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