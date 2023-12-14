using System;
using UnityEngine;

namespace Characters.Player
{
    [Serializable]
    public class PlayerAnimationData
    {
        [SerializeField] private string weaponParameterName = "@Weapon";
        [SerializeField] private string targetLockParameterName = "@TargetLock";
        [SerializeField] private string attackParameterName = "Attack";
        [SerializeField] private string comboAttackParameterName = "ComboAttack";
        
        [SerializeField] private string idleParameterName = "Idle";
        [SerializeField] private string moveParameterName = "Move";
        [SerializeField] private string moveFastParameterName = "MoveFast";
        [SerializeField] private string crouchParameterName = "Crouch";
        [SerializeField] private string slideParameterName = "Slide";
        
        [SerializeField] private string jumpParameterName = "Jump";
        [SerializeField] private string fallParameterName = "Fall";
        
        [SerializeField] private string moveDirXParameterName = "MoveDirX";
        [SerializeField] private string moveDirYParameterName = "MoveDirY";
        
        [SerializeField] private string defenceParameterName = "Defence";
        [SerializeField] private string comboParameterName = "Combo";
        [SerializeField] private string equipParameterName = "Equip";
        [SerializeField] private string unEquipParameterName = "UnEquip";
        
        [SerializeField] private string dieParameterName = "Die";
        [SerializeField] private string hitParameterName = "Hit";
        [SerializeField] private string dieIndexParameterName = "DieIndex";
        
        public int TargetLockParameterName { get; private set; }
        public int WeaponParameterHash { get; private set; }
        
        public int AttackParameterHash { get; private set; }
        public int ComboAttackParameterHash { get; private set; }
        
        public int IdleParameterHash { get; private set; }
        public int MoveParameterHash { get; private set; }
        public int MoveFastParameterHash { get; private set; }
        public int CrouchParameterHash { get; private set; }
        public int SlideParameterHash { get; private set; }

        public int JumpParameterHash { get; private set; }
        public int FallParameterHash { get; private set; }
        
        public int MoveDirXParameterHash { get; private set; }
        public int MoveDirYParameterHash { get; private set; }
        
        public int DefenceParameterHash { get; private set; }
        public int ComboParameterHash { get; private set; }
        public int EquipParameterHash { get; private set; }
        public int UnEquipParameterHash { get; private set; }
        
        public int DieParameterHash { get; private set; }
        public int DieIndexParameterHash { get; private set; }
        public int HitParameterHash { get; private set; }

        public void Initialize()
        {
            TargetLockParameterName = Animator.StringToHash(targetLockParameterName);
            AttackParameterHash = Animator.StringToHash(attackParameterName);
            ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
            
            IdleParameterHash = Animator.StringToHash(idleParameterName);
            MoveParameterHash = Animator.StringToHash(moveParameterName);
            MoveFastParameterHash = Animator.StringToHash(moveFastParameterName);
            CrouchParameterHash = Animator.StringToHash(crouchParameterName);
            SlideParameterHash = Animator.StringToHash(slideParameterName);
            
            JumpParameterHash = Animator.StringToHash(jumpParameterName);
            FallParameterHash = Animator.StringToHash(fallParameterName);

            WeaponParameterHash = Animator.StringToHash(weaponParameterName);
            
            MoveDirXParameterHash = Animator.StringToHash(moveDirXParameterName);
            MoveDirYParameterHash = Animator.StringToHash(moveDirYParameterName);

            DefenceParameterHash = Animator.StringToHash(defenceParameterName);
            ComboParameterHash = Animator.StringToHash(comboParameterName);
            
            EquipParameterHash = Animator.StringToHash(equipParameterName);
            UnEquipParameterHash = Animator.StringToHash(unEquipParameterName);

            DieParameterHash = Animator.StringToHash(dieParameterName);
            DieIndexParameterHash = Animator.StringToHash(dieIndexParameterName);

            HitParameterHash = Animator.StringToHash(hitParameterName);
        }
    }
}