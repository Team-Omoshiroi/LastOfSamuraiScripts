using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<BaseItem> Items = new List<BaseItem>();
    public List<BaseConsumable> consumeItems = new List<BaseConsumable>();
    public List<BaseWeapone> EquipmentItems = new List<BaseWeapone>();

    public Transform[] ItemContent;
    public GameObject InventoryItemPrefab;
    public Toggle EnableRemove;
    public Button InvenButton;
    public GameObject UiInventory;

   
   public  EquipmentSlot[] equipmentSlots;

    public GameObject UIInteractive;
    public TextMeshProUGUI TxTInteractive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Init();
        InvenButton.onClick.AddListener(ToggleInventory);
    }

    // 인벤토리 UI 초기화
    void Init()
    {
        UIInteractive.SetActive(false);
        EnableRemove.isOn = false;
    }

    // 인벤토리 토글
    void ToggleInventory()
    {
        
        ContentListUpdate();
        UiInventory.SetActive(!UiInventory.activeSelf);
    }

    // 아이템 추가
    public void Add(BaseItem item)
    {
        switch (item.ItemType)
        {
            case eItemType.Consumable:
                consumeItems.Add(item as BaseConsumable);
                break;
            case eItemType.Weapon:
                EquipmentItems.Add(item as BaseWeapone);
                break;
            default:
                Items.Add(item);
                break;
        }
        ContentListUpdate();
    }

    // 아이템 제거
    public void Remove(BaseItem item)
    {
        if (item == null)
        {
            Debug.LogError("NullItem");
            return;
        }

        switch (item.ItemType)
        {
            case eItemType.Consumable:
                consumeItems.Remove(item as BaseConsumable);
                break;
            case eItemType.Weapon:
                EquipmentItems.Remove(item as BaseWeapone);
                break;
            default:
                Items.Remove(item);
                break;
        }


    }

    // 인벤토리 UI 리스트 업데이트
    public void ContentListUpdate()
    {
        UpdateItemListUI(0, Items);
        UpdateItemListUI(1, consumeItems);
        UpdateItemListUI(2, EquipmentItems);
    }

    // 인벤토리 슬롯 생성
    public GameObject InstantiateSlot(Transform parent)
    {
        return Instantiate(InventoryItemPrefab, parent);
    }

    // UI 리스트 업데이트
    private void UpdateItemListUI<T>(int categoryIndex, List<T> items) where T : BaseItem
    {
        // 카테고리 내의 아이템 수를 업데이트된 리스트의 크기와 일치시키기
        while (ItemContent[categoryIndex].childCount < items.Count)
        {
            InstantiateSlot(ItemContent[categoryIndex]);
        }

        // 기존 아이템을 업데이트 또는 비활성화
        for (int i = 0; i < ItemContent[categoryIndex].childCount; i++)
        {
            if (i < items.Count)
            {
                Transform child = ItemContent[categoryIndex].GetChild(i);
                child.gameObject.SetActive(true);
                SetItemUI(child.gameObject, items[i]);
            }
            else
            {
                ItemContent[categoryIndex].GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    
      

 

   public void EqipSlotAdd(InventoryItemContoller itemController)
    {
        BaseWeapone item = itemController.item as BaseWeapone;
        if (item == null) return; // 아이템 타입 체크

        foreach (var eqipSlot in equipmentSlots)
        {
            if (eqipSlot.eqipType == item.EqipType)
            {
             
                var itemIcon = itemController.gameObject.transform.Find("BorderImage/ItemIcon").GetComponent<Image>();
                GameObject equippedItem = Instantiate(itemIcon.gameObject, eqipSlot.slotTransform);
               

                break; 
            }
        }
    }


    public void EqipSlotRemove(InventoryItemContoller itemController)
    {
        BaseWeapone item = itemController.item as BaseWeapone;
        if (item == null) return; // 아이템 타입 체크

        foreach (var eqipSlot in equipmentSlots)
        {
            if (eqipSlot.eqipType == item.EqipType)
            {
                // 해당 슬롯에서 모든 자식 오브젝트를 순회
                foreach (Transform child in eqipSlot.slotTransform)
                {                                      
                        Destroy(child.gameObject); // 오브젝트 제거
                        break; // 해당 아이템을 찾고 제거했으므로 루프 탈출                   
                }
                break; // 일치하는 슬롯을 찾았으므로 루프 탈출
            }
        }
    }



    // 인벤토리 아이템 초기화 

    // 아이템 UI 설정
    private void SetItemUI(GameObject obj, BaseItem item)
    {
        var inventoryItemController = obj.GetComponent<InventoryItemContoller>();
        var itemName = obj.transform.Find("BorderImage/ItemIcon/NameBG/ItemName").GetComponent<TMP_Text>();
        var itemIcon = obj.transform.Find("BorderImage/ItemIcon").GetComponent<Image>();
        var removeButton = obj.transform.Find("BtnRemove").GetComponent<Button>();
        var eqipIcon = obj.transform.Find("BorderImage/ItemIcon/EqipIcon");

        itemName.text = item.ItemName;
        itemIcon.sprite = item.ItemIcon;
        // eqipIcon.gameObject.SetActive(inventoryItemController.IsEqip);
        removeButton.gameObject.SetActive(EnableRemove.isOn);

        if (inventoryItemController != null)
        {
            inventoryItemController.SetItem(item);
        }
        else
        {
            Debug.LogError("개체에 InventoryItemContoller 없음.");
        }
    }

    // 삭제 버튼 활성화/비활성화
    public void EnableItemsRemoveAll()
    {
        for (int i = 0; i < ItemContent.Length; i++)
        {
            EnableItemsRemove(i);
        }
    }

    // 아이템 별 삭제 버튼 활성화/비활성화
    public void EnableItemsRemove(int num)
    {
        foreach (Transform item in ItemContent[num])
        {
            item.Find("BtnRemove").gameObject.SetActive(EnableRemove.isOn);
        }
    }

    [System.Serializable]
    public class EquipmentSlot
    {
        public eEqipType eqipType;
        public Transform slotTransform;
        public EquipmentSlot(eEqipType type, Transform trans)
        {
            eqipType = type;
            slotTransform = trans;
        }
    }
}
