using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal 
{
   protected bool Unsubscribe;
    public Quest Quest { get; set; }
    public string Description { get; set; } // 퀘스트목표에 대한설명
    public bool Completed { get; set; }   //퀘스트완료
    public int CurrentAmount { get; set; }  //현재수행목표양   
    public int RequiredAmount { get; set; } // 목표수행양

    public virtual void Init()
    {
        //모든 초기화항목 
    }
   public void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Completed = true; // 목표 달성
            Unsubscribe = true;
            Complete(); // 퀘스트 완료
          
        }
    }

    public void Complete()
    {     
        Quest.CheckGoals();
        Completed = true;
        EventNotifier.Instance.Notify($"퀘스트 완료");         
    }
    

}
