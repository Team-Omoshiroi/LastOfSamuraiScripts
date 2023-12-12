using Characters.Player.StateMachines.TargetLock;
using Characters.StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player.StateMachines
{
    public class PlayerBaseState : IState
    {
        protected readonly PlayerStateMachine playerStateMachine;
        protected readonly Player player;
        protected readonly TargetLockModule targetLockModule;
        protected readonly Animator animator;
        protected readonly PlayerInputActions.PlayerActions playerActions;
        protected readonly PlayerAnimationData playerAnimationData;
        protected bool animationMovement;
        protected bool isDodging;
        protected bool rotateBreak;

        protected PlayerBaseState(PlayerStateMachine stateMachine)
        {
            playerStateMachine = stateMachine;
            player = playerStateMachine.Player;
            targetLockModule = playerStateMachine.Player.TargetLockModule;
            animator = playerStateMachine.Player.Animator;
            playerActions = playerStateMachine.Player.InputModule.PlayerActions;
            playerAnimationData = playerStateMachine.Player.AnimationData;
            animationMovement = true;
            rotateBreak = false;
        }

        public virtual void Enter()
        {
            AddInputActionsCallbacks();
        }

        public virtual void Exit()
        {
            RemoveInputActionsCallbacks();
        }
        
        public virtual void HandleInput()
        {
            ReadLookInput();
            ReadCrouchInput();
            ReadMoveInput();
        }
        
        public virtual void PhysicsUpdate()
        {
            Move();
        }
        
        public virtual void Update()
        {
            GetMoveDirection();
        }

        public virtual void LateUpdate() { }

        protected virtual void AddInputActionsCallbacks()
        {
            playerActions.Move.canceled += OnMoveCanceled;
            playerActions.Move.started += OnMoveStarted;
            playerActions.MoveFast.performed += OnMoveFastPerformed;
            playerActions.MoveFast.canceled += OnMoveFastCanceled;
            playerActions.Slide.performed += OnSlidePerformed;
            // playerActions.Jump.started += OnJumpStarted;
            
            playerActions.Weapon.started += OnWeaponStarted;
            playerActions.TargetLock.started += OnTargetLockStarted;
            
            playerActions.Defence.performed += OnDefencePerformed;
            playerActions.Defence.canceled += OnDefenceCanceled;
            
            playerActions.Attack.performed += OnAttackPerformed;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            
            playerActions.Move.canceled -= OnMoveCanceled;
            playerActions.Move.started -= OnMoveStarted;
            playerActions.MoveFast.performed -= OnMoveFastPerformed;
            playerActions.MoveFast.canceled -= OnMoveFastCanceled;
            playerActions.Slide.performed -= OnSlidePerformed;
            // playerActions.Jump.started -= OnJumpStarted;
            
            playerActions.Weapon.started -= OnWeaponStarted;
            playerActions.TargetLock.started -= OnTargetLockStarted;

            playerActions.Defence.performed -= OnDefencePerformed;
            
            playerActions.Attack.performed -= OnAttackPerformed;
        }

        protected virtual void OnMoveCanceled(InputAction.CallbackContext obj) { }
        protected virtual void OnMoveStarted(InputAction.CallbackContext obj) { }
        protected virtual void OnSlidePerformed(InputAction.CallbackContext obj) { }
        protected virtual void OnMoveFastPerformed(InputAction.CallbackContext obj) { }
        protected virtual void OnMoveFastCanceled(InputAction.CallbackContext obj) { }
        protected virtual void OnWeaponStarted(InputAction.CallbackContext obj) { }
        protected virtual void OnTargetLockStarted(InputAction.CallbackContext obj) { }
        protected virtual void OnAttackPerformed(InputAction.CallbackContext obj) { }
        protected virtual void OnDefencePerformed(InputAction.CallbackContext obj) { }
        protected virtual void OnDefenceCanceled(InputAction.CallbackContext obj) { }
        protected virtual void OnJumpStarted(InputAction.CallbackContext obj) { }
    
        /// <summary>
        /// Crouch (웅크리기) 키 입력을 받아 Stand 상태에서 Crouch 상태로 전환합니다.
        /// </summary>
        private void ReadCrouchInput()
        {
            var currentCrouch = player.Animator.GetFloat(player.AnimationData.CrouchParameterHash);

            if (player.InputModule.PlayerActions.Crouch.triggered)
            {
                if (playerStateMachine.IsCrouch == false)
                {
                    playerStateMachine.TargetCrouch = 0;
                    playerStateMachine.IsCrouch = true;
                }
                else
                {
                    playerStateMachine.TargetCrouch = 1;
                    playerStateMachine.IsCrouch = false;
                }
            }
            
            currentCrouch = Mathf.Lerp(currentCrouch, playerStateMachine.TargetCrouch, 0.2f);
            
            player.Animator.SetFloat(player.AnimationData.CrouchParameterHash, currentCrouch);
        }

        /// <summary>
        /// PlayerInputAction으로부터 Move의 Vector2 값을 가지고 옵니다.
        /// (11.24 Inho)
        /// </summary>
        private void ReadMoveInput()
        {
            playerStateMachine.MoveInput = player.InputModule.PlayerActions.Move.ReadValue<Vector2>();
        }
        
        /// <summary>
        /// PlayerInputAction으로부터 Look의 Vector2 값을 가지고 옵니다.
        /// (11.24 Inho)
        /// </summary>
        private void ReadLookInput()
        {
            playerStateMachine.LookInput = player.InputModule.PlayerActions.Look.ReadValue<Vector2>();
        }
        
        /// <summary>
        /// Animator에서 Root Animation의 deltaPosition을 가지고 와
        /// CharacterController.Move() 메서드를 호출합니다.
        /// (11.24 Inho)
        /// </summary>
        private void Move()
        {
            switch (animationMovement)
            {
                case true:
                    player.Controller.Move((player.Animator.deltaPosition) * Time.deltaTime);
                    break;
                case false:
                    player.Controller.Move(
                        (playerStateMachine.MoveDirection + playerStateMachine.Player.ForceReceiver.Movement)
                        * Time.deltaTime
                    );
                    break;
            }
        }
        
        /// <summary>
        /// MainCameraTransform 기준으로 이동 방향을 구해 MoveDirection에 할당합니다.
        /// (11.26 Inho)
        /// </summary>
        private void GetMoveDirection()
        {
            var forward = playerStateMachine.MainCameraTransform.forward;
            var right = playerStateMachine.MainCameraTransform.right;
        
            forward.y = 0;
            right.y = 0;
        
            forward.Normalize();
            right.Normalize();
        
            playerStateMachine.MoveDirection = forward * playerStateMachine.MoveInput.y + right * playerStateMachine.MoveInput.x;
        }
        
        /// <summary>
        /// 파라미터로 받은 animationHash에 대응하는 애니메이션에 SetBool()을 통해 실행시킵니다.
        /// (11.24 Inho)
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StartAnimationWithBool(int animationHash)
        {
            player.Animator.SetBool(animationHash, true);
        }
        
        /// <summary>
        /// 파라미터로 받은 animationHash에 대응하는 애니메이션에 SetBool()을 통해 중단시킵니다.
        /// (11.24 Inho)
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StopAnimationWithBool(int animationHash)
        {
            player.Animator.SetBool(animationHash, false);
        }
        
        /// <summary>
        /// 파라미터로 받은 animationHash에 대응하는 애니메이션에 SetTrigger() 메서드를 실행합니다.
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StartAnimationWithTrigger(int animationHash)
        {
            player.Animator.SetBool(animationHash, true);
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
        
        // /// <summary>
        // /// moveDirection을 인자로 받아 해당 방향으로 PlayerTransform을 회전합니다.
        // /// </summary>
        // /// <param name="moveDirection"></param>
        // private void RotatePlayer(Vector3 moveDirection)
        // {
        //     if (moveDirection == Vector3.zero) return;
        //
        //     var playerTransform = player.transform;
        //     var targetRotation = Quaternion.LookRotation(moveDirection);
        //     
        //     playerTransform.rotation = 
        //         Quaternion.Slerp(
        //             playerTransform.rotation,
        //             targetRotation, 
        //             PlayerStateMachine.RotationDamping * Time.deltaTime);
        // }

        // /// <summary>
        // /// CombatCamera를 실행시킵니다.
        // /// true - CombatCamera
        // /// false - NormalCamera
        // /// (11.27 Inho)
        // /// </summary>
        // /// <param name="transformCombatCamera"></param>
        // protected void SetCombatCamera(bool transformCombatCamera)
        // {
        //     // PlayerStateMachine.Player.NormalCamera.SetActive(!transformCombatCamera);
        //     // PlayerStateMachine.Player.CombatCamera.SetActive(transformCombatCamera);
        //     // PlayerStateMachine.Player.Aim.SetActive(transformCombatCamera);
        // }
    }
}