namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockAttackState : PlayerTargetLockState
    {
        public PlayerTargetLockAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.AttackParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.AttackParameterHash);
        }
    }
}