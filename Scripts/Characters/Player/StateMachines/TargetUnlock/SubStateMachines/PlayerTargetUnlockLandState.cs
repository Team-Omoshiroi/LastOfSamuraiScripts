using UnityEngine;

namespace Characters.Player.StateMachines.TargetUnlock.SubStateMachines
{
    public class PlayerTargetUnlockLandState : PlayerTargetUnlockState
    {
        public PlayerTargetUnlockLandState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(player.AnimationData.FallParameterHash);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(player.AnimationData.FallParameterHash);

            animationMovement = true;
        }

        public override void Update()
        {
            base.Update();

            if (player.Controller.isGrounded)
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
            }
        }
        
        private bool IsCheckGrounded()
        {
            if (player.Controller.isGrounded) return true;

            var layerMask = 1 << LayerMask.NameToLayer("Ground");
            var position = player.transform.position;
            var ray = new Ray(position + Vector3.up * 0.1f, Vector3.down);
            const float maxDistance = 1.5f;
            
            Debug.DrawRay(position + Vector3.up * 0.1f, Vector3.down * maxDistance, Color.red);
            
            return Physics.Raycast(ray, maxDistance, layerMask);
        }
    }
}