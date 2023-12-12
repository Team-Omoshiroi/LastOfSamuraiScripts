using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseConsumableSO", menuName = "Item/BaseConsumableSO")]
public class BaseConsumable : BaseItem, IDestroyableItem, IItemAction
{
    [SerializeField]
    private List<ModifierData> modifiersData = new List<ModifierData>();


    public string ActionName
    {
        get
        {
            if (itemType == eItemType.Consumable)
            {
                return "consume";
            }
            else
            {
                return "undefined"; // or handle the undefined case appropriately
            }

        }
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
