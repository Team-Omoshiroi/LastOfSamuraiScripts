using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slayer :Quest
{

    void Start()
    {
        QuestName = "Slayer";
        Description = " Kill a bunch of stuff.";
        ItemReward = ItemDataBase.Instance.GetItem(7);
        ExperienceReward = 100;

        Goals.Add(new GetItemGoal(this, 1, "kill 5 Bear",false,0,2));
      
        Goals.ForEach(g => g.Init());
        EventNotifier.Instance.Notify($"퀘스트 시작");
    }


}
