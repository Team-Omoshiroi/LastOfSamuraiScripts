using UnityEngine;

namespace Characters.Player.StateMachines
{
    public class PlayerDieState : PlayerBaseState
    {
        public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            
            SetDieMotion();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.DieParameterHash);
        }

        private void SetDieMotion()
        {
            var dieIndex = Random.Range(0, 11);
            
            Debug.Log($"{dieIndex}");
            
            playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.DieIndexParameterHash, dieIndex);
        }
        
        protected override void AddInputActionsCallbacks() { }

        protected override void RemoveInputActionsCallbacks(){}
    }
}