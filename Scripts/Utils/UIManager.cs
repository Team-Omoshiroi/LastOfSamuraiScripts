using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : PlainSingleton<UIManager>
{
    private Dictionary<Type, GameObject> _prefabs;
    private LinkedList<UIBase> OpenList;
    private LinkedList<UIBase> HideList;

    //TODO
    //Data�� �����ϱ�
    public float UISize = 1.0f;// { get => GameManager.Data.UISize; }
    public float FontSize = 1.0f;// { get => GameManager.Data.FontSizeMultiplier; }
    public float UIRemainTime = 30.0f;// { get => GameManager.Data.UIRemainTime; }

    private string _prefabPath = "Prefabs/UI/";

    private Canvas _canvas;
    public Canvas UIRoot
    {
        get
        {
            //if (_canvas == null)
            //    if (GameObject.Find("_UI").TryGetComponent(out _canvas))
            //        return _canvas;
            //    else
            //        return null;
            return _canvas;
        }
        set
        {
            _canvas = value;
        }
    }

    public UIManager()
    {
        _prefabs = new Dictionary<Type, GameObject>();
        OpenList = new LinkedList<UIBase>();
        HideList = new LinkedList<UIBase>();
        //LoadUIPrefabs();
    }

    /// <summary>
    /// Open ����Ʈ�� ù��°�� ��ġ�� UI�� Hide�ϸ�, �̹� Hide�� ��쿡�� �ƹ��͵� ���� ����
    /// </summary>
    public void HideTopUI()
    {
        if (OpenList.Count > 0)
        {
            HideUI(OpenList.First.Value);
        }
    }

    /// <summary>
    /// �ش� ��ũ��Ʈ�� �ٿ��� �������� �ҷ���, Hide�� ���ӿ�����Ʈ�� �ִ� ��쿡�� �ش� ���ӿ�����Ʈ�� ������
    /// </summary>
    /// <typeparam name="T">�����鿡 �پ��ִ� Ŭ����</typeparam>
    /// <param name="root">�θ� canvas/UI�� �ǹ���</param>
    /// <returns>�ش� �������� ������ null�� ������</returns>
    public T ShowUI<T>(string prefabPath = null, RectTransform root = null) where T : UIBase
    {
        var open = GetHideUI<T>();
        if (open != null)
        {
            HideList.Remove(open);
            if (root == null)
            {
                if (UIRoot != null)
                    open.transform.SetParent(UIRoot.transform);
                else
                    open.transform.parent = null;
            }
            else
                open.transform.SetParent(root);

            open.gameObject.SetActive(true);

            open.AddActAtHide(() => AddtoHideList(open));
            open.AddActAtClose(() => DeleteInList(open));
            return open;
        }

        if (!_prefabs.ContainsKey(typeof(T)))
        {
            if (prefabPath == null)
                LoadUIPrefab(typeof(T).ToString());
            else
                LoadUIPrefab(prefabPath);
        }

        var prefab = _prefabs[typeof(T)];
        if (prefab != null)
        {
            GameObject obj;
            if (root == null)
            {
                if (UIRoot != null)
                    obj = GameObject.Instantiate(prefab, UIRoot.transform);
                else
                    obj = GameObject.Instantiate(prefab);
            }
            else
                obj = GameObject.Instantiate(prefab, root);
            var uiClass = obj.GetComponent<UIBase>();

            OpenList.AddFirst(uiClass);
            obj.SetActive(true);

            uiClass.AddActAtHide(() => AddtoHideList(uiClass));
            uiClass.AddActAtClose(() => DeleteInList(uiClass));
            return uiClass as T;
        }
        else
            return null;
    }

    private void AddtoHideList<T>(T ui) where T : UIBase
    {
        HideList.AddLast(ui);
        OpenList.Remove(ui);
        OpenList.AddLast(ui);
    }

    private void DeleteInList<T>(T ui) where T : UIBase
    {
        if (IsHide(ui))
        {
            OpenList.Remove(ui);
            HideList.Remove(ui);
        }
        else
        {
            OpenList.Remove(ui);
        }
    }

    /// <summary>
    /// ���� ������Ʈ�� ����
    /// </summary>
    public void CloseUI<T>(T target) where T : UIBase
    {
        target.CloseUI();
    }

    /// <summary>
    /// ���� ������Ʈ�� ��Ȱ��ȭ
    /// </summary>
    public void HideUI<T>(T target) where T : UIBase
    {
        target.HideUI();
    }

    /// <summary>
    /// �ش� UI�� Open List�� �ִ��� Ȯ���ϴ� �޼ҵ��, Hide ������ �˷����� �ʴ´�.
    /// Ȱ��ȭ ���´� activeInHierarchy�� ���� �� �� �ְ�, ����ϱ� ���ؼ��� ShowUI(eUIType type)�� �θ��� �ȴ�.
    /// </summary>
    /// <returns>ã�� �� ������ null�� ������</returns>
    public T GetOpenUI<T>(T search) where T : UIBase
    {
        foreach (var ui in OpenList)
        {
            if (ui == search)
                return ui as T;
        }
        return null;
    }

    /// <summary>
    /// ���� �ش� UI Type�� Open List�� �ִ��� Ȯ���ϴ� �޼ҵ��, Hide ������ �˷����� �ʴ´�.
    /// Ȱ��ȭ ���´� activeInHierarchy�� ���� �� �� �ְ�, ����ϱ� ���ؼ��� ShowUI(eUIType type)�� �θ��� �ȴ�.
    /// </summary>
    /// <returns>ã�� �� ������ null�� ������</returns>
    public T GetOpenUI<T>() where T : UIBase
    {
        LinkedListNode<UIBase> ui = OpenList.First;
        while (ui != null)
        {
            if (ui.Value is T)
                return ui.Value as T;
            ui = ui.Next;
        }
        return null;
    }

    /// <summary>
    /// �ش� UI�� Hide List�� �ִ��� Ȯ���ϴ� �޼ҵ�
    /// </summary>
    /// <returns>ã�� �� ������ null�� ������</returns>
    public T GetHideUI<T>(T search) where T : UIBase
    {
        foreach (var ui in HideList)
        {
            if (ui == search)
                return ui as T;
        }
        return null;
    }

    /// <summary>
    /// Hide�� �ش� UI Type�� Hide ����Ʈ�� �ִ��� Ȯ���ϴ� �޼ҵ�
    /// </summary>
    /// <returns>ã�� �� ������ null�� ������</returns>
    public T GetHideUI<T>() where T : UIBase
    {
        LinkedListNode<UIBase> ui = HideList.First;
        while (ui != null)
        {
            if (ui.Value is T)
                return ui.Value as T;
            ui = ui.Next;
        }
        return null;
    }

    /// <summary>
    /// Open List�� �ִ� ��� UI ���ӿ�����Ʈ�� �����Ѵ�.
    /// </summary>
    public void CloseAllOpenUI()
    {
        foreach (var ui in OpenList)
        {
            ui.CloseUI();
        }
        OpenList.Clear();
        HideList.Clear();
    }

    /// <summary>
    /// Hide List�� �ִ� ��� UI ���ӿ�����Ʈ�� �����Ѵ�.
    /// </summary>
    public void CloseAllHideUI()
    {
        foreach (var ui in HideList)
        {
            ui.CloseUI();
            OpenList.Remove(ui);
        }
        HideList.Clear();
    }

    /// <summary>
    /// �ش� UI type�� Open List�� ���ԵǾ� �ֳ� Ȯ���Ѵ�. IsHide�� ��쿡 ���� �� �����ϴ�.
    /// </summary>
    public bool IsOpen<T>() where T : UIBase
    {
        foreach (var ui in OpenList)
        {
            if (ui is T)
                return true;
        }
        return false;
    }

    /// <summary>
    /// �ش� UI�� Open List�� ���ԵǾ� �ֳ� Ȯ���Ѵ�.
    /// </summary>
    public bool IsOpen<T>(T target) where T : UIBase
    {
        foreach (var ui in OpenList)
        {
            if (ui == target)
                return true;
        }
        return false;
    }

    /// <summary>
    /// �ش� UI type�� Hide List�� ���ԵǾ� �ֳ� Ȯ���Ѵ�.
    /// </summary>
    public bool IsHide<T>() where T : UIBase
    {
        foreach (var ui in HideList)
        {
            if (ui is T)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Hide List�� ���ԵǾ� �ֳ� Ȯ���Ѵ�.
    /// </summary>
    public bool IsHide<T>(T target) where T : UIBase
    {
        foreach (var ui in HideList)
        {
            if (ui == target)
                return true;
        }
        return false;
    }

    private void LoadUIPrefab(string name)
    {
        var obj = Resources.Load<GameObject>(_prefabPath + name);
        if (obj != null)
        {
            var type = obj.GetComponent<UIBase>().GetType();
            _prefabs.Add(type, obj);
            Debug.Log($"{obj.name}({_prefabPath}/{obj.name}) is loaded.");
        }
    }
}
