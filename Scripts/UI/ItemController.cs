using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
   public BaseItem item;

    [SerializeField]
    private Vector3 rotationSpeed = new Vector3(0, 0, 10); 

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime*10);
    }
}
