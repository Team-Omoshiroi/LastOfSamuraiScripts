using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/Eqip")]
public class CharacterStatEqipModifierSo : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        InventoryItemContoller item = character.GetComponent<InventoryItemContoller>();

        if (item != null)
        {
           
            if (item.IsEqip)
            {
                item.Equip((int)val);
              
            }
            else
            {
                item.DeEqip((int)val);
             
            }
        }
    }
}
