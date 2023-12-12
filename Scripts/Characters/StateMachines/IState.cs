namespace Characters.StateMachines
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void HandleInput();
        public void PhysicsUpdate();
        public void Update();
        public void LateUpdate();
    }
}