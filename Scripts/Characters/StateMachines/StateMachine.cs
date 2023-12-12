using UnityEngine;

namespace Characters.StateMachines
{
    public abstract class StateMachine
    {
        public IState currentState;

        public void ChangeState(IState newState)
        {
            currentState?.Exit();

            currentState = newState;

            currentState?.Enter();
        }

        public void HandleInput()
        {
            currentState?.HandleInput();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void PhysicsUpdate()
        {
            currentState?.PhysicsUpdate();
        }
        
        public void LateUpdate()
        {
            currentState?.LateUpdate();
        }
    }
}