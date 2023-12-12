using UnityEngine;

namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockSlideState : PlayerTargetLockAttackState
    {
        public PlayerTargetLockSlideState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            isDodging = true;
            SetDodgeDirection();
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
                playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
            }
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveState);
            }
        }
        
        protected override void AddInputActionsCallbacks() { }

        protected override void RemoveInputActionsCallbacks() { }

        protected override void UpdateVectorInAnimator() { }

        private void SetDodgeDirection()
        {
            if (playerStateMachine.MoveInput.x > 0f)
            {
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, 1);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, 0);
            }
            else if (playerStateMachine.MoveInput.x < 0f)
            {
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, -1);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, 0);
            }
            else if (playerStateMachine.MoveInput.y > 0f)
            {
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, 0);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, 1);
            }
            else if (playerStateMachine.MoveInput.y < 0f)
            {
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, 0);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, -1);
            }
        }

        // protected override void UpdateVectorInAnimator()
        // {
        //     if (IsDodging == false)
        //     {
        //         base.UpdateVectorInAnimator();
        //     }
        // }
    }
}