using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; set; }
    public GameObject UIdialoguePanel;

    public string npcName;
    public List<string> dialogueLines = new List<string>();
    Button ContinueButton;
    TextMeshProUGUI dialogueText, nameText;
    int dialogueIndex;

    private void Awake()
    {
        ContinueButton = UIdialoguePanel.transform.Find("BtnContinue")?.GetComponent<Button>();
        dialogueText = UIdialoguePanel.transform.Find("Talk")?.GetComponent<TextMeshProUGUI>();
        nameText = UIdialoguePanel.transform.Find("NameBox")?.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (ContinueButton == null || dialogueText == null || nameText == null)
        {
            Debug.LogError("DialogueSystem: 필요한 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        ContinueButton.onClick.AddListener(()=> { ContinueDialogue(); });
        UIdialoguePanel.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 인스턴스 유지
        }
    }
  


    public void AddNewDialogue(string[] lines, string npcName)
    {
        dialogueIndex = 0;
        dialogueLines = new List<string>();
        foreach (string line in lines)
        {
            dialogueLines.Add(line);
        }
        this.npcName = npcName;
     
        createDialogue(); 
    }

    public void createDialogue()
    {
        dialogueText.text = dialogueLines[dialogueIndex];
        nameText.text = npcName;
        UIdialoguePanel.SetActive(true);
    }

    public void ContinueDialogue()
    {
        if(dialogueIndex <dialogueLines.Count-1)
        {
            dialogueIndex++;
            dialogueText.text = dialogueLines[dialogueIndex];
        }
        else
        {
            UIdialoguePanel.SetActive(false);
        }
    }
}
