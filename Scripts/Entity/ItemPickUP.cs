using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickUp : MonoBehaviour
{
    // 아이템 픽업 이벤트 정의
    public static event Action<BaseItem> OnItemPickedUp;
    public float interactionRadius = 1.0f;

    public void Interect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (TryInteractWithItem(hitCollider) || TryInteractWithNPC(hitCollider))
            {
                SetUIInteractive(false);
                break;
            }
               
        }
    }

    private bool TryInteractWithItem(Collider collider)
    {
        var itemController = collider.GetComponent<ItemController>();
        if (itemController != null)
        {
            PickUp(itemController.item);
            Destroy(collider.gameObject);
            return true;
        }
        return false;
    }

    private bool TryInteractWithNPC(Collider collider)
    {
        var npc = collider.GetComponent<NPC>();
        if (npc != null)
        {
            npc.Interact();
            return true;
        }
        return false;
    }

    private void PickUp(BaseItem item)
    {
        // 아이템 획득 처리
        InventoryManager.Instance.Add(item);
        EventNotifier.Instance.Notify($"{item.ItemName} 획득");

        // 아이템 획득 이벤트 발행
        OnItemPickedUp?.Invoke(item);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Item"))
        {
            SetUIInteractive(true, "E : 줍기");
        }
        else if (collider.CompareTag("NPC"))
        {
            SetUIInteractive(true, "E : 대화");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Item") || collider.CompareTag("NPC"))
        {
            SetUIInteractive(false);
        }
    }

    void SetUIInteractive(bool set, string Interactive = null)
    {
        InventoryManager.Instance.UIInteractive.SetActive(set);
        InventoryManager.Instance.TxTInteractive.text = Interactive;
    }
}
