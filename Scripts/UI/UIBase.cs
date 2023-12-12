using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected Action ActAtHide;
    protected Action ActAtClose;

    /// <summary>
    /// 주의!!! Action에 기본적으로 UIManger의 hide list에 추가하는 메소드가 들어가 있으므로, 덮어씌우지 말고 추가해서 사용할 것.
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddActAtHide(Action action) { ActAtHide += action; }
    /// <summary>
    /// 주의!!! Action에 기본적으로 UIManger의 open list에서 지우는 메소드가 들어가 있으므로, 덮어씌우지 말고 추가해서 사용할 것.
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddActAtClose(Action action) { ActAtClose += action; }

    protected virtual void OnDisable()
    {
        //Invoke(nameof(CloseUI), UIManager.Instance.UIRemainTime);
    }

    protected virtual void OnEnable()
    {
        CancelInvoke();
    }

    protected virtual void InitialSize()
    {
        gameObject.transform.localScale = Vector3.one * UIManager.Instance.UISize;
    }

    public virtual void RefreshSize()
    {
        InitialSize();
    }

    /// <summary>
    /// 게임 오브젝트를 삭제
    /// </summary>
    public virtual void CloseUI()
    {
        ActAtClose?.Invoke();
        Destroy(gameObject);
        ActAtClose = null;
    }

    /// <summary>
    /// 게임 오브젝트를 비활성화
    /// </summary>
    public virtual void HideUI()
    {
        if (!UIManager.Instance.IsHide(this))
        {
            ActAtHide?.Invoke();
            if (gameObject != null)
                gameObject.SetActive(false);
            ActAtHide = null;
        }
    }
}
