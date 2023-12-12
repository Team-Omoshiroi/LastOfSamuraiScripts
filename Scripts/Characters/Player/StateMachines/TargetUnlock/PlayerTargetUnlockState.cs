using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player.StateMachines.TargetUnlock
{
    public class PlayerTargetUnlockState : PlayerBaseState
    {
        public PlayerTargetUnlockState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

        public override void Update()
        {
            base.Update();
            
            if (rotateBreak) return;
            RotatePlayer();
        }

        #region InputActionMethod
        /// 걷기 키가 입력되지 않으면 IdleState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
        }
        
        /// <summary>
        /// 걷기 키가 입력되면 MoveState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveStarted(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveState);
        }
        
        /// <summary>
        /// 달리기 키를 꾹 누르고 있는 상태라면 RunState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveFastPerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveFastState);
        }

        /// <summary>
        /// 걷기 키와 달리기 키가 입력되지 않으면 IdleState로,
        /// 달리기 키만 입력되지 않으면 MoveState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveFastCanceled(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.MoveInput == Vector2.zero)
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
            }
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveState);
            }
        }
        
        protected override void OnSlidePerformed(InputAction.CallbackContext obj)
        {
            // if (!PlayerStateMachine.Player.Animator.GetBool(PlayerStateMachine.Player.AnimationData.WeaponParameterName)) return;
            if (isDodging) return;
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockSlideState);
        }
        
        protected override void OnJumpStarted(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetUnlockJumpState);
        }
        
        /// <summary>
        /// Weapon 키 입력 시 WeaponOn / OffState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnWeaponStarted(InputAction.CallbackContext obj)
        {
            if (animator.GetBool(playerAnimationData.WeaponParameterHash))
            {
                var unEquipKatana = Random.Range(0, 5);
                animator.SetFloat(playerAnimationData.UnEquipParameterHash, unEquipKatana);

                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockWeaponOffState);     
            }
            else
            {
                var equipKatana = Random.Range(0, 2);
                animator.SetFloat(playerAnimationData.EquipParameterHash, equipKatana);

                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockWeaponOnState);
            }
        }
        
        protected override void OnTargetLockStarted(InputAction.CallbackContext obj)
        {
            playerStateMachine.Player.TargetLockModule.currentTarget = playerStateMachine.Player.TargetLockModule.ScanNearBy();

            if (playerStateMachine.Player.TargetLockModule.currentTarget)
            {
                playerStateMachine.Player.TargetLockModule.FoundTarget();
            }
            else
            {
                playerStateMachine.Player.TargetLockModule.ResetTarget();
                return;
            }
            
            if (animator.GetBool(playerAnimationData.IdleParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
            }
            
            if (animator.GetBool(playerAnimationData.MoveParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveState);
            }
            
            if (animator.GetBool(playerAnimationData.MoveFastParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveFastState);
            }
        }
        
        protected override void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.WeaponParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockComboAttackState);   
            }
        }
        
        protected override void OnDefencePerformed(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.WeaponParameterHash))
            {
                animator.SetBool(playerAnimationData.DefenceParameterHash, true);   
            }
        }
        
        protected override void OnDefenceCanceled(InputAction.CallbackContext obj)
        {
            animator.SetBool(playerAnimationData.DefenceParameterHash, false);
        }
        
        #endregion
        
        /// <summary>
        /// moveDirection을 인자로 받아 해당 방향으로 PlayerTransform을 회전합니다.
        /// </summary>
        private void RotatePlayer()
        {
            if (playerStateMachine.MoveDirection == Vector3.zero) return;

            var playerTransform = player.transform;
            var targetRotation = Quaternion.LookRotation(playerStateMachine.MoveDirection);
            
            playerTransform.rotation = 
                Quaternion.Slerp(
                    playerTransform.rotation,
                    targetRotation, 
                    playerStateMachine.RotationDamping * Time.deltaTime);
        }
    }
}