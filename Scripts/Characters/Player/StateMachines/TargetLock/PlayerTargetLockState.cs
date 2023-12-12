using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player.StateMachines.TargetLock
{
    public class PlayerTargetLockState : PlayerBaseState
    {
        public PlayerTargetLockState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StartAnimationWithBool(playerStateMachine.Player.AnimationData.TargetLockParameterName);
        }
        
        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(playerStateMachine.Player.AnimationData.TargetLockParameterName);
        }
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            UpdateVectorInAnimator();
        }

        #region InputActionMethod
        
        protected override void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
        }
        
        protected override void OnMoveStarted(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveState);
        }
        
        protected override void OnSlidePerformed(InputAction.CallbackContext obj)
        {
            if (isDodging) return;
            playerStateMachine.ChangeState(playerStateMachine.TargetLockSlideState);
        }
        
        protected override void OnMoveFastPerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveFastState);
        }
        
        protected override void OnMoveFastCanceled(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.MoveInput == Vector2.zero)
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
            }
            else
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveState);
            }
        }
        
        protected override void OnWeaponStarted(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.WeaponParameterHash))
            {
                var unEquipKatana = Random.Range(0, 5);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.UnEquipParameterHash, unEquipKatana);

                playerStateMachine.ChangeState(playerStateMachine.TargetLockWeaponOffState);    
            }
            else
            {
                var equipKatana = Random.Range(0, 2);
                playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.EquipParameterHash, equipKatana);

                playerStateMachine.ChangeState(playerStateMachine.TargetLockWeaponOnState);
            }
        }
        
        protected override void OnTargetLockStarted(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.TargetLockModule.currentTarget)
            {
                playerStateMachine.Player.TargetLockModule.ResetTarget();
            }
            
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.IdleParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockIdleState);
            }
            
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.MoveParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveState);
            }
            
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.MoveFastParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetUnlockMoveFastState);
            }
        }
        
        protected override void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.WeaponParameterHash))
            {
                playerStateMachine.ChangeState(playerStateMachine.TargetLockComboAttackState);
            }
        }
        
        protected override void OnDefencePerformed(InputAction.CallbackContext obj)
        {
            if (playerStateMachine.Player.Animator.GetBool(playerStateMachine.Player.AnimationData.WeaponParameterHash))
            {
                playerStateMachine.Player.Animator.SetBool(playerStateMachine.Player.AnimationData.DefenceParameterHash, true);
            }
        }
        
        protected override void OnDefenceCanceled(InputAction.CallbackContext obj)
        {
            playerStateMachine.Player.Animator.SetBool(playerStateMachine.Player.AnimationData.DefenceParameterHash, false);
        }
        
        #endregion
        
        // /// <summary>
        // /// deltaMouse 값을 받아 Cinemachine과 플레이어를 회전시킵니다.
        // /// (11.24 Inho)
        // /// </summary>
        // private void Look()
        // {
        //     var minXLook = playerStateMachine.Player.MinXLook;
        //     var maxXLook = playerStateMachine.Player.MaxXLook;
        //     var lookSensitivity = playerStateMachine.Player.LookSensitivity;
        //     var mouseDelta = playerStateMachine.LookInput;
        //     
        //     playerStateMachine.Player.cameraXRotation += mouseDelta.y * lookSensitivity;
        //     playerStateMachine.Player.cameraXRotation = Mathf.Clamp(playerStateMachine.Player.cameraXRotation, minXLook, maxXLook);
        //     
        //     playerStateMachine.Player.FollowTarget.localEulerAngles = new Vector3(-playerStateMachine.Player.cameraXRotation, 0, 0);
        //     playerStateMachine.Player.transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
        // }
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateVectorInAnimator()
        {
            playerStateMachine.SmoothVector = Vector2.Lerp( playerStateMachine.SmoothVector,  playerStateMachine.MoveInput, playerStateMachine.SmoothingVec);
            
            playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirXParameterHash, playerStateMachine.SmoothVector.x);
            
            playerStateMachine.Player.Animator.SetFloat(playerStateMachine.Player.AnimationData.MoveDirYParameterHash, playerStateMachine.SmoothVector.y);
        }
    }
}