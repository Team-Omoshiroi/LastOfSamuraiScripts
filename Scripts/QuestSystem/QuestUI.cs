using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText; // 퀘스트 제목을 표시할 Text
    [SerializeField] private TextMeshProUGUI descriptionText; // 퀘스트 설명을 표시할 Text
 
    [SerializeField] private TextMeshProUGUI rewardText; // 퀘스트 보상을 표시할 Text

    // 퀘스트 정보를 UI에 표시하는 메서드
    public void DisplayQuest(Quest quest)
    {
        titleText.text = quest.name; 
        descriptionText.text = quest.Description; 
    
        rewardText.text = quest.ItemReward.ItemName;

        
        if (quest.Completed)
        {
            //todo 퀘스트가 완료됬을때 
        }
    }

  
    public void OnQuestStarted(Quest quest)
    {
        DisplayQuest(quest);
    }
}
