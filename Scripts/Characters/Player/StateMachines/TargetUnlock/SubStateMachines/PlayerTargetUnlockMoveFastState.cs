namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockMoveFastState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockMoveFastState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

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