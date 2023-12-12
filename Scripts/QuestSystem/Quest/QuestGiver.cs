using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : NPC
{
    public bool AssignedQuest { get;  set; }
    public bool Helped { get; set; }
    [SerializeField]
    private GameObject quests;
    [SerializeField]
    private string questType;
    public Quest Quest { get; set; }

 
    public override void Interact()
    {
        if(!AssignedQuest && !Helped)  // 퀘스트도받지않고  완료하지도않음 
        {
            base.Interact();
            AssignQuest();
        }
        else if(AssignedQuest && !Helped)  // 퀘스트를받았지만 완료하지않음 
        {
            CheckQuest();
        }
        else  //퀘스트를받았고  완료까지 한'상태'
        {
            //  도와줘서고마웠어요 같은 대사출력 
            DialogueSystem.Instance.AddNewDialogue(new string[] { " 사과 고마웠어 ","할말있어?" }, name);
        }
    }
    void AssignQuest()
    {
        AssignedQuest = true;
        Quest = (Quest)quests.AddComponent(System.Type.GetType(questType));
       
    }
    void CheckQuest()
    {
        if (Quest != null && Quest.Completed)
        {
            Quest.GiveReward();
            Helped = true;
            AssignedQuest = false;       
            DialogueSystem.Instance.AddNewDialogue(new string[] { "사과 고마워 여기 핼멧를 보상으로줄께  " },name);
        }
        else
        {             
            DialogueSystem.Instance.AddNewDialogue(new string[] { "아직 사과를 다모아오지않았어 .. " }, name);
        }
    }
}
