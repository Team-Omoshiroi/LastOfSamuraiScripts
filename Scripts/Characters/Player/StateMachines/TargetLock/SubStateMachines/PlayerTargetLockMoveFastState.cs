namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockMoveFastState : PlayerTargetLockState
    {
        public PlayerTargetLockMoveFastState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.MoveFastParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.MoveFastParameterHash);
        }
    }
}