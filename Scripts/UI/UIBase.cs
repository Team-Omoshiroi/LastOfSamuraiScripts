using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected Action ActAtHide;
    protected Action ActAtClose;

    /// <summary>
    /// ����!!! Action�� �⺻������ UIManger�� hide list�� �߰��ϴ� �޼ҵ尡 �� �����Ƿ�, ������� ���� �߰��ؼ� ����� ��.
    /// </summary>
    /// <param name="action"></param>
    public virtual void AddActAtHide(Action action) { ActAtHide += action; }
    /// <summary>
    /// ����!!! Action�� �⺻������ UIManger�� open list���� ����� �޼ҵ尡 �� �����Ƿ�, ������� ���� �߰��ؼ� ����� ��.
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
    /// ���� ������Ʈ�� ����
    /// </summary>
    public virtual void CloseUI()
    {
        ActAtClose?.Invoke();
        Destroy(gameObject);
        ActAtClose = null;
    }

    /// <summary>
    /// ���� ������Ʈ�� ��Ȱ��ȭ
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
