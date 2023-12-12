using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemContoller : MonoBehaviour
{
    // 인벤토리 아이템을 참조
    public BaseItem item;
    [SerializeField]
    public GameObject UIEqip;


    public Button RemoveButton;


    public Button BtnUse;


    public bool IsEqip = true;


    private void Start()
    {

        BtnUse.onClick.AddListener(() => { UseItem(); });
        RemoveButton.onClick.AddListener(() => { DropItem(); });

    }

    // 아이템 제거 메서드
    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);
        Destroy(gameObject);
    }

   public void DropItem()
    {
        if (IsEqip == true || item.ItemType != eItemType.Weapon)
        {
            InventoryManager.Instance.Remove(item);


            Vector3 randomDropPosition = GetRandomPositionAroundPlayer();
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

            Instantiate(item.ObjectPrefab, randomDropPosition, rotation);

            Destroy(gameObject);
        }
    
    }


    private Vector3 GetRandomPositionAroundPlayer(float radius = 1f)
    {
        // 플레이어 위치에서 랜덤한 방향과 거리를 계산합니다.
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0.2f; 
  
   
        Vector3 dropPosition = PlayerStatusController.Instance.dropRoot.transform.position + randomDirection;

        return dropPosition;
    }


    public void Equip(int val)
    {
        BaseWeapone weaponItem = item as BaseWeapone; // 형변환 추가

        if (weaponItem != null)
        {
            foreach (var equipSlot in InventoryManager.Instance.equipmentSlots)
            {
                if (equipSlot.eqipType == weaponItem.EqipType) // 형변환된 객체의 EqipType 사용
                {
                    // 이미 장착된 아이템이 있다면 제거
                    if (equipSlot.slotTransform.childCount > 0)
                    {
                        InventoryItemContoller equippedItemController = equipSlot.slotTransform.GetChild(0).GetComponent<InventoryItemContoller>();
                        if (equippedItemController != null)
                        {
                            InventoryManager.Instance.EqipSlotRemove(equippedItemController);
                        }
                    }

                    // 새 아이템 장착
                    Eqip(val);
                    break;
                }
            }
        }
    }
    public void Eqip(int val)
{
    PlayerStatusController.Instance.AP += val;
    IsEqip = false; // 아이템이 장착되었음을 나타냅니다.
    UIEqip.SetActive(true); // 장착 아이콘 활성화
    //PlayerStatusController.Instance.TxtAP.text = $"ATK:{PlayerStatusController.Instance.AP}";

    // 장착된 아이템 UI 업데이트를 위한 호출
    InventoryManager.Instance.EqipSlotAdd(this);
}

    // 장비 해제 메서드
    public void DeEqip(int val)
    {
        PlayerStatusController.Instance.AP -= val;
        IsEqip = true; // 아이템이 해제되었음을 나타냄
        UIEqip.SetActive(false); // 장착 아이콘 비활성화
        //PlayerStatusController.Instance.TxtAP.text = $"ATK:{PlayerStatusController.Instance.AP}";

        // 장착 해제된 아이템 UI 업데이트를 위한 호출
        InventoryManager.Instance.EqipSlotRemove(this);
    }

    // 아이템 사용 메서드
    public void UseItem()
    {

        if (item is IItemAction itemAction)
        {               
          itemAction.PerformAction(gameObject); // 아이템의 행동 수행
        }
        else
        {
            Debug.LogError("이 아이템은 사용할 수 없습니다.");
        }
    }

    // 아이템 초기화 
    public void SetItem<T>(T newItem) where T : BaseItem
    {
        item = newItem;

    }
}

