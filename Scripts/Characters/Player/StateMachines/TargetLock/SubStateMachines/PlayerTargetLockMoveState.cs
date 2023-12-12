namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockMoveState : PlayerTargetLockState
    {
        public PlayerTargetLockMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.MoveParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.MoveParameterHash);
        }
    }
}