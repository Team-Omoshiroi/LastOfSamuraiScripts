using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseWeaponeSO", menuName = "Item/BaseWeaponeSO")]
public class BaseWeapone : BaseItem, IItemAction
{


    public eEqipType eqipType; // 'eEqipType' 유형으로 선언
    public eEqipType EqipType { get { return eqipType; } } // 수정된 게터


    [SerializeField]
    private List<ModifierData> modifiersData = new List<ModifierData>();

    [Header("WeaponData")]
    [SerializeField]
    [Tooltip("공격력")]
    protected int weaponAP;

    public string ActionName => throw new NotImplementedException();

    public void Equip()
    {
        PlayerStatusController.Instance.AP += weaponAP;
    }

    public void Dequip()
    {
        PlayerStatusController.Instance.AP -= weaponAP;
    }

    public bool PerformAction(GameObject character)
    {
        foreach (ModifierData data in modifiersData)
        {
            data.statModifier.AffectCharacter(character, data.value);
        }
        return true;
    }



    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}
