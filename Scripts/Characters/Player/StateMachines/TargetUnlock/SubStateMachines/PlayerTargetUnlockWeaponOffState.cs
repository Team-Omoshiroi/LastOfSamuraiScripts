using UnityEngine;

namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockWeaponOffState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockWeaponOffState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
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

            if (!(0.8f < animationState.normalizedTime)) return;

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