using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemGoal : Goal
{
    public int EnemyID { get; set; }


    public GetItemGoal(Quest quest,int enumyId,string Description,bool completrd,int CurrentAmount, int requiredAmount )
    {
      
        this.Quest = quest;
        this.EnemyID = enumyId; 
        this.Description = Description;
        this.Completed = completrd;
        this.CurrentAmount = CurrentAmount; 
        this.RequiredAmount = requiredAmount;
     
    }
   
    public override void Init()
    {
        base.Init();
        ItemPickUp.OnItemPickedUp += GetItem;
    }


    void GetItem(BaseItem item)
    {
        if (item.ItemID == this.EnemyID)
        {
            this.CurrentAmount++;
            Evaluate();
            
            if (Unsubscribe == true)
            {
                ItemPickUp.OnItemPickedUp -= GetItem;
            }

        }
    }
       
    

}
