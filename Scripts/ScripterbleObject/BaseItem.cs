using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseItem : ScriptableObject
{
	[Header("BaseItemData")]
	[SerializeField] protected Sprite itemIcon;
	public Sprite ItemIcon { get { return itemIcon; } }

	[SerializeField] protected GameObject objectPrefab;
	public GameObject ObjectPrefab { get { return objectPrefab; } }

	[SerializeField] protected int itemID;
	public int ItemID { get { return itemID; } }

	[SerializeField] protected string itemName;
	public string ItemName { get { return itemName; } }

	[SerializeField] protected string itemDescription;
	public string ItemDescription { get { return itemDescription; } }

	[SerializeField] protected eItemType itemType;
	public eItemType ItemType { get { return itemType; } }

	[SerializeField] protected int itemMaxStack;
	public int ItemMaxStack { get { return itemMaxStack; } }

	[SerializeField] protected bool isStackable;
	public bool IsStackable { get { return isStackable; } }
}
