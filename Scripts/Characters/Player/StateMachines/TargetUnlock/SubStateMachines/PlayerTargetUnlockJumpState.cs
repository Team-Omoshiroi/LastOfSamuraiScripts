using UnityEngine;

namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockJumpState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.JumpParameterHash);
            
            animationMovement = false;
            
            playerStateMachine.JumpForce = playerStateMachine.Player.Data.AirData.JumpForce;
            player.ForceReceiver.Jump(playerStateMachine.JumpForce);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.JumpParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!(playerStateMachine.Player.Controller.velocity.y <= 0)) return;
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockLandState);
        }
    }
}