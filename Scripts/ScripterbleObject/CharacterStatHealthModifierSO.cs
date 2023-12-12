using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/AddHP")]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {

      var item =  character.GetComponent<InventoryItemContoller>();
         if(item!=null)
        {
            PlayerStatusController.Instance.IncreaseHealth((int)val);
            item.RemoveItem();
        }
     
    }
}