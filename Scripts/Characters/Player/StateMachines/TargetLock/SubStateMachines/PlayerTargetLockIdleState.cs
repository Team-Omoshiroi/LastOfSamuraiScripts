namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockIdleState : PlayerTargetLockState
    {
        public PlayerTargetLockIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.IdleParameterHash);
            
            playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, 0);
            playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, 0);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.IdleParameterHash);
        }
    }
}