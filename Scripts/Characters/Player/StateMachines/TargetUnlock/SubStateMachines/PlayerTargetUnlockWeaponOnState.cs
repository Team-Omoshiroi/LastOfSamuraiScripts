using UnityEngine;

namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockWeaponOnState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockWeaponOnState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            rotateBreak = true;
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.WeaponParameterHash);
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
            
            if (!animationState.IsTag("Equip")) return;
            
            if (!(0.8f < animationState.normalizedTime)) return;

            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
        }
        
        protected override void AddInputActionsCallbacks() { }

        protected override void RemoveInputActionsCallbacks() { }
    }
}