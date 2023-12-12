using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase Instance { get; set; }
    [field:SerializeField]
    public  List<BaseItem> Items { get; set; }
    void Start()
    {
        if(Instance!=null && Instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public BaseItem GetItem(int Itemid)
    {
        foreach(BaseItem item in Items)
        {
            if(item.ItemID == Itemid)
            {
              
                return item;
            }
        }
        return null;
    }

 
}
