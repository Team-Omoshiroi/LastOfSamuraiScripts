namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockMoveState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

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