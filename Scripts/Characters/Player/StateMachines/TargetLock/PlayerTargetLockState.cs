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
        /// <summary>
        /// 걷기 키가 입력되지 않으면 CombatIdleState 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockIdleState);
        }
        
        /// <summary>
        /// 걷기 키가 입력되면 CombatMoveState 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnMoveStarted(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveState);
        }
        
        /// <summary>
        /// 키를 짧게 눌렀다 떼면 CombatDodgeState로 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected override void OnSlidePerformed(InputAction.CallbackContext obj)
        {
            if (isDodging) return;
            playerStateMachine.ChangeState(playerStateMachine.TargetLockSlideState);
        }
        
        protected override void OnMoveFastPerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockMoveFastState);
        }

        /// <summary>
        /// 걷기 키와 달리기 키가 입력되지 않으면 CombatIdleState,
        /// 달리기 키만 입력되지 않으면 CombatMoveState 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
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
        
        /// <summary>
        /// Combat 키 입력 시 NormalIdleState 전환합니다.
        /// </summary>
        /// <param name="obj"></param>
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
        
        protected override void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.ChangeState(playerStateMachine.TargetLockComboAttackState);
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

        protected override void OnDefencePerformed(InputAction.CallbackContext obj)
        {
            playerStateMachine.Player.Animator.SetBool(playerStateMachine.Player.AnimationData.DefenceParameterHash, true);
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
       
       protected static float GetNormalizedTime(Animator animator, string tag)
       {
           var currentInfo = animator.GetCurrentAnimatorStateInfo(0);
           var nextInfo = animator.GetNextAnimatorStateInfo(0);

           if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
           {
               return nextInfo.normalizedTime;
           }
           else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
           {
               return currentInfo.normalizedTime;
           }
           else
           {
               return 0f;
           }
       }
    }
}