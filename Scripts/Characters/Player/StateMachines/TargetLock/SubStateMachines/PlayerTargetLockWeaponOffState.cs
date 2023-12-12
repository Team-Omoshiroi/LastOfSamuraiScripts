using UnityEngine;

namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockWeaponOffState : PlayerTargetLockState
    {
        public PlayerTargetLockWeaponOffState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            rotateBreak = true;
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.WeaponParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            rotateBreak = false;
        }

        public override void Update()
        {
            base.Update();
            
            var animationState = playerStateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
            
            if (!animationState.IsTag("UnEquip")) return;

            if (animationState.normalizedTime < 0.8f) return;

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
    }
}