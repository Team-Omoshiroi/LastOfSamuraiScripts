using Characters.Player.StateMachines.TargetLock.SubStateMachines;
using UnityEngine;

namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockSlideState : PlayerTargetLockAttackState
    {
        public PlayerTargetUnlockSlideState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            isDodging = true;
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.SlideParameterHash);
        }

        public override void Exit()
        {
            isDodging = false;
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.SlideParameterHash);
        }

        public override void Update()
        {
            var animationState = playerStateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);

            if (!animationState.IsTag("Slide")) return;

            if (!(animationState.normalizedTime >= 0.3f)) return;
            if (playerStateMachine.MoveInput == Vector2.zero)
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
            }
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveState);
            }
        }
        
        protected override void AddInputActionsCallbacks() { }

        protected override void RemoveInputActionsCallbacks() { }
    }
}