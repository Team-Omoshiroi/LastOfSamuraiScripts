using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    private List<InventoryItem> OtherItems; // 인벤토리 내 아이템 목록


   


    [field: SerializeField]
    public int Size { get; set; } = 10; // 인벤토리의 최대 크기

    // 아이템 추가, 삭제 등 인벤토리 업데이트 시 발생하는 이벤트
    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

    // 인벤토리 초기화: 최대 크기만큼 빈 아이템 슬롯을 생성
    public void Initialize()
    {
        OtherItems = new List<InventoryItem>();
      

        for (int i = 0; i < Size; i++)
        {
            OtherItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    // 인벤토리 아이템 목록 설정
    public void SetInventoryItems(List<InventoryItem> items)
    {
        OtherItems = items;

    }

    public List<InventoryItem> GetItemsOfType(eItemType type)
    {
        List<InventoryItem> filteredItems = new List<InventoryItem>();
        foreach (var item in OtherItems)
        {
            if (item.item != null && item.item.ItemType == type)
            {
                filteredItems.Add(item);
            }
        }
        return filteredItems;
    }

    // 특정 인덱스에 아이템 삽입
    public void InsertItem(int index, InventoryItem item)
    {
        if (index >= 0 && index < OtherItems.Count)
        {
            OtherItems[index] = item;         
        }
    }


    // 아이템 추가: 스택 가능 여부에 따라 처리
    public int AddItem(BaseItem item, int quantity)
    {
        // 스택 불가능 아이템일 경우, 각각의 아이템을 개별 슬롯에 추가
        if (item.IsStackable == false)
        {
            while (quantity > 0 && IsInventoryFull() == false)
            {
                quantity -= AddItemToFirstFreeSlot(item, 1);
            }
            InformAboutChange();
            return quantity;
        }

        // 스택 가능 아이템일 경우, 스택 처리
        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }

    // 첫 번째 빈 슬롯에 아이템 추가
    private int AddItemToFirstFreeSlot(BaseItem item, int quantity)
    {
        InventoryItem newItem = new InventoryItem
        {
            item = item,
            quantity = quantity,
        };

        for (int i = 0; i < OtherItems.Count; i++)
        {
            if (OtherItems[i].IsEmpty)
            {
                OtherItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    // 인벤토리가 가득 찼는지 확인
    private bool IsInventoryFull() => OtherItems.Where(item => item.IsEmpty).Any() == false;

    // 스택 가능 아이템 추가 로직
    private int AddStackableItem(BaseItem item, int quantity)
    {
        for (int i = 0; i < OtherItems.Count; i++)
        {
            // 현재 슬롯이 비어있으면 다음 슬롯으로
            if (OtherItems[i].IsEmpty)
                continue;

            // 같은 아이템 ID를 가진 아이템이 있는 경우 스택 처리
            if (OtherItems[i].item.ItemID == item.ItemID)
            {
                // 현재 슬롯에 추가할 수 있는 아이템 수
                int amountPossibleToTake =
                       OtherItems[i].item.ItemMaxStack - OtherItems[i].quantity;

                // 스택 처리
                if (quantity > amountPossibleToTake)
                {
                    OtherItems[i] = OtherItems[i]
                        .ChangeQuantity(OtherItems[i].item.ItemMaxStack);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    OtherItems[i] = OtherItems[i]
                        .ChangeQuantity(OtherItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }

        // 아직 남은 아이템을 새 슬롯에 추가
        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.ItemMaxStack);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }
        return quantity;
    }

    // 특정 인덱스의 아이템 제거
    public void RemoveItem(int itemIndex, int amount)
    {
        if (OtherItems.Count > itemIndex)
        {
            if (OtherItems[itemIndex].IsEmpty)
                return;

            // 제거 후 남은 수량 계산
            int reminder = OtherItems[itemIndex].quantity - amount;
            if (reminder <= 0)
                OtherItems[itemIndex] = InventoryItem.GetEmptyItem();
            else
                OtherItems[itemIndex] = OtherItems[itemIndex]
                    .ChangeQuantity(reminder);

            InformAboutChange();
        }
    }

    // 현재 인벤토리 상태 반환
    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

        for (int i = 0; i < OtherItems.Count; i++)
        {
            if (!OtherItems[i].IsEmpty)
            {
                returnValue[i] = OtherItems[i];
            }
        }
        return returnValue;
    }

    // 특정 인덱스의 아이템 반환
    public InventoryItem GetItemAt(int itemIndex)
    {
        return OtherItems[itemIndex];
    }

    // 아이템 추가 - 오버로딩 메서드
    public void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }

    // 두 아이템의 위치를 바꾸는 메서드
    public void SwapItems(int itemIndex_1, int itemIndex_2)
    {
        InventoryItem item1 = OtherItems[itemIndex_1];
        OtherItems[itemIndex_1] = OtherItems[itemIndex_2];
        OtherItems[itemIndex_2] = item1;
        InformAboutChange();
    }

    // 인벤토리 상태 변경을 알리는 메서드
    private void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }
}

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public BaseItem item;

    public bool IsEmpty => item == null; // 아이템이 비어있는지 여부

    // 아이템의 수량을 변경
    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    // 빈 아이템 인스턴스 생성
    public static InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null,
            quantity = 0,
        };
}
