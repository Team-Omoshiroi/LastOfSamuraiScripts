using Characters.Player.StateMachines.TargetLock;
using Characters.Player.StateMachines.TargetLock.SubStateMachines;
using Characters.Player.StateMachines.TargetUnlock;
using Characters.Player.StateMachines.TargetUnlock.SubStateMachines;
using Characters.StateMachines;
using UnityEngine;

namespace Characters.Player.StateMachines
{
    public class PlayerStateMachine : StateMachine
    {
        public Player Player { get; }

        // TargetUnlock
        public PlayerTargetUnlockState TargetUnlockState { get; }
        
        public PlayerTargetUnlockIdleState TargetUnlockIdleState { get; }
        public PlayerTargetUnlockMoveState TargetUnlockMoveState { get; }
        public PlayerTargetUnlockMoveFastState TargetUnlockMoveFastState { get; }
        public PlayerTargetUnlockSlideState TargetUnlockSlideState { get; }
        public PlayerTargetUnlockAttackState TargetUnlockAttackState { get; }
        public PlayerTargetUnlockComboAttackState TargetUnlockComboAttackState { get; }
        
        public PlayerTargetUnlockWeaponOnState TargetUnlockWeaponOnState { get; }
        public PlayerTargetUnlockWeaponOffState TargetUnlockWeaponOffState { get; }
        
        // TargetLock
        public PlayerTargetLockState TargetLockState { get; }
        
        public PlayerTargetLockIdleState TargetLockIdleState { get; }
        public PlayerTargetLockMoveState TargetLockMoveState { get; }
        public PlayerTargetLockMoveFastState TargetLockMoveFastState { get; }
        public PlayerTargetLockSlideState TargetLockSlideState { get; }
        public PlayerTargetLockAttackState TargetLockAttackState { get; }
        public PlayerTargetLockComboAttackState TargetLockComboAttackState { get; }
        
        public PlayerTargetLockWeaponOnState TargetLockWeaponOnState { get; }
        public PlayerTargetLockWeaponOffState TargetLockWeaponOffState { get; }

        public PlayerTargetUnlockJumpState TargetUnlockJumpState { get; }
        public PlayerTargetUnlockLandState TargetUnlockLandState { get; }

        // Crouch
        public float TargetCrouch { get; set; }
        public bool IsCrouch { get; set; } = false;
        
        public Vector2 MoveInput { get; set; } // 움직임 방향
        public Vector2 SmoothVector { get; set; } // Animator MoveDir 보간
        public float SmoothingVec { get; set; } = 0.1f;
        public Vector2 LookInput { get; set; } // 카메라 방향
        public Vector3 LookDirection { get; set; } 
        public Vector3 MoveDirection { get; set; }
        public float RotationDamping { get; private set; }
        public bool IsAttacking { get; set; }
        public int ComboIndex { get; set; }
        public float JumpForce { get; set; }

        public Transform MainCameraTransform { get; set; }

        public PlayerStateMachine(Player player)
        {
            Player = player;

            // TargetUnlock
            TargetUnlockState = new PlayerTargetUnlockState(this);
            
            TargetUnlockIdleState = new PlayerTargetUnlockIdleState(this);
            TargetUnlockMoveState = new PlayerTargetUnlockMoveState(this);
            TargetUnlockMoveFastState = new PlayerTargetUnlockMoveFastState(this);
            TargetUnlockSlideState = new PlayerTargetUnlockSlideState(this);

            TargetUnlockAttackState = new PlayerTargetUnlockAttackState(this);
            TargetUnlockComboAttackState = new PlayerTargetUnlockComboAttackState(this);
            
            TargetUnlockWeaponOnState = new PlayerTargetUnlockWeaponOnState(this);
            TargetUnlockWeaponOffState = new PlayerTargetUnlockWeaponOffState(this);
            
            // TargetLock
            TargetLockState = new PlayerTargetLockState(this);
            
            TargetLockIdleState = new PlayerTargetLockIdleState(this);
            TargetLockMoveState = new PlayerTargetLockMoveState(this);
            TargetLockMoveFastState = new PlayerTargetLockMoveFastState(this);
            TargetLockSlideState = new PlayerTargetLockSlideState(this);
            
            TargetLockAttackState = new PlayerTargetLockAttackState(this);
            TargetLockComboAttackState = new PlayerTargetLockComboAttackState(this);
            
            TargetLockWeaponOnState = new PlayerTargetLockWeaponOnState(this);
            TargetLockWeaponOffState = new PlayerTargetLockWeaponOffState(this);
            
            // etc
            TargetUnlockJumpState = new PlayerTargetUnlockJumpState(this);
            TargetUnlockLandState = new PlayerTargetUnlockLandState(this);
            
            if (Camera.main != null) MainCameraTransform = Camera.main.transform;

            RotationDamping = player.Data.GroundData.BaseRotationDamping;
        }
    }
}