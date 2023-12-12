using ScriptableObjects.Scripts;
using UnityEngine.InputSystem;

namespace Characters.Player.StateMachines.TargetLock.SubStateMachines
{
    public class PlayerTargetLockComboAttackState : PlayerTargetLockAttackState
    {
        private bool alreadyApplyCombo;
        private AttackInfoData attackInfoData;

        public PlayerTargetLockComboAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.ComboAttackParameterHash);

            alreadyApplyCombo = false;

            var comboIndex = playerStateMachine.ComboIndex;
            attackInfoData = playerStateMachine.Player.Data.AttackData.GetAttackInfo(comboIndex);
            playerStateMachine.Player.Animator.SetInteger(playerStateMachine.Player.AnimationData.ComboParameterHash, comboIndex);
        }

        public override void Exit()
        {
            base.Exit();

            playerStateMachine.IsAttacking = false;
            
            if (!alreadyApplyCombo)
            {
                playerStateMachine.ComboIndex = 0;
            }
            
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.ComboAttackParameterHash);
        }

        public override void Update()
        {
            base.Update();

            var normalizedTime = GetNormalizedTime(playerStateMachine.Player.Animator, "Attack");
            
            if ((attackInfoData.MinComboTransitionTime - 0.1f) <= normalizedTime && normalizedTime <= attackInfoData.MaxComboTransitionTime)
            {
                if (alreadyApplyCombo && (attackInfoData.MinComboTransitionTime <= normalizedTime))
                {
                    playerStateMachine.ComboIndex = attackInfoData.ComboStateIndex;
                    playerStateMachine.ChangeState(playerStateMachine.TargetLockComboAttackState);
                }
                else
                {
                    TryComboAttack();
                }
            }
            else if (attackInfoData.MaxComboTransitionTime < normalizedTime)
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
            }
        }

        protected override void AddInputActionsCallbacks()
        {
            playerStateMachine.Player.InputModule.PlayerActions.Slide.performed += OnSlidePerformed;
            
            playerStateMachine.Player.InputModule.PlayerActions.TargetLock.started += OnTargetLockStarted;
            
            playerStateMachine.Player.InputModule.PlayerActions.Defence.performed += OnDefencePerformed;
            playerStateMachine.Player.InputModule.PlayerActions.Defence.canceled += OnDefenceCanceled;
            
            playerStateMachine.Player.InputModule.PlayerActions.Attack.performed += OnAttackPerformed;
        }
        
        protected override void RemoveInputActionsCallbacks()
        {
            playerStateMachine.Player.InputModule.PlayerActions.Slide.performed -= OnSlidePerformed;
            
            playerStateMachine.Player.InputModule.PlayerActions.TargetLock.started -= OnTargetLockStarted;
            
            playerStateMachine.Player.InputModule.PlayerActions.Defence.performed -= OnDefencePerformed;
            playerStateMachine.Player.InputModule.PlayerActions.Defence.canceled -= OnDefenceCanceled;
            
            playerStateMachine.Player.InputModule.PlayerActions.Attack.performed -= OnAttackPerformed;
        }
        
        private void TryComboAttack()
        {
            if (alreadyApplyCombo) return;

            if (attackInfoData.ComboStateIndex == -1) return;

            if (!playerStateMachine.IsAttacking) return;
            
            alreadyApplyCombo = true;
        }

        protected override void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.IsAttacking = true;
        }
    }
}