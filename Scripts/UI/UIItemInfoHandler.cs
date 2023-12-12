using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItemInfoHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject itemInfoUI; // ������ ������ ǥ���� UI ������Ʈ

    private InventoryItemContoller inventoryItemController;
    [SerializeField]
    private TextMeshProUGUI discription;


   
    private void Awake()
    {
        inventoryItemController = GetComponent<InventoryItemContoller>();
    }
    private void Start()
    {

        UpdateItemInfoUI(inventoryItemController.item);
    }


    void UpdateItemInfoUI(BaseItem item)
    {
        if (item == null) return;
        var textComponent = discription.GetComponent<TextMeshProUGUI>();
        textComponent.text = item.ItemDescription.ToString();
        itemInfoUI.SetActive(true);
        if (discription != null)
        {
            discription.text = textComponent.text;
        }    
      
    }

}
