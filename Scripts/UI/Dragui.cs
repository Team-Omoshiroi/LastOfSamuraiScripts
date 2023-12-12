using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragui : MonoBehaviour, IPointerDownHandler, IDragHandler

{///<summary>
 /// �巡�� �ٿ� DragWindow ��ũ��Ʈ�� �߰��ϰ�
 /// window ������ �����̰� ���� â�� �ν����Ϳ� �巡�� �Ѵ�.
 ///</summary>

    public RectTransform window; //Drag Move Window
    private Vector2 downPosition;

    public void OnPointerDown(PointerEventData data)
    {
        downPosition = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 offset = data.position - downPosition;
        downPosition = data.position;

        window.anchoredPosition += offset;
    }

}
