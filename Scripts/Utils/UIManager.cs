using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : PlainSingleton<UIManager>
{
    private Dictionary<Type, GameObject> _prefabs;
    private LinkedList<UIBase> OpenList;
    private LinkedList<UIBase> HideList;

    //TODO
    //Data랑 연결하기
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
    /// Open 리스트의 첫번째에 위치한 UI를 Hide하며, 이미 Hide된 경우에는 아무것도 하지 않음
    /// </summary>
    public void HideTopUI()
    {
        if (OpenList.Count > 0)
        {
            HideUI(OpenList.First.Value);
        }
    }

    /// <summary>
    /// 해당 스크립트가 붙여진 프리펩을 불러옴, Hide된 게임오브젝트가 있는 경우에는 해당 게임오브젝트를 리턴함
    /// </summary>
    /// <typeparam name="T">프리펩에 붙어있는 클래스</typeparam>
    /// <param name="root">부모 canvas/UI를 의미함</param>
    /// <returns>해당 프리펩이 없으면 null을 리턴함</returns>
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
    /// 게임 오브젝트를 삭제
    /// </summary>
    public void CloseUI<T>(T target) where T : UIBase
    {
        target.CloseUI();
    }

    /// <summary>
    /// 게임 오브젝트를 비활성화
    /// </summary>
    public void HideUI<T>(T target) where T : UIBase
    {
        target.HideUI();
    }

    /// <summary>
    /// 해당 UI가 Open List에 있는지 확인하는 메소드로, Hide 유무는 알려주지 않는다.
    /// 활성화 상태는 activeInHierarchy를 보면 알 수 있고, 사용하기 위해서는 ShowUI(eUIType type)를 부르면 된다.
    /// </summary>
    /// <returns>찾을 수 없으면 null을 리턴함</returns>
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
    /// 열린 해당 UI Type이 Open List에 있는지 확인하는 메소드로, Hide 유무는 알려주지 않는다.
    /// 활성화 상태는 activeInHierarchy를 보면 알 수 있고, 사용하기 위해서는 ShowUI(eUIType type)를 부르면 된다.
    /// </summary>
    /// <returns>찾을 수 없으면 null을 리턴함</returns>
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
    /// 해당 UI가 Hide List에 있는지 확인하는 메소드
    /// </summary>
    /// <returns>찾을 수 없으면 null을 리턴함</returns>
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
    /// Hide된 해당 UI Type이 Hide 리스트에 있는지 확인하는 메소드
    /// </summary>
    /// <returns>찾을 수 없으면 null을 리턴함</returns>
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
    /// Open List에 있는 모든 UI 게임오브젝트를 삭제한다.
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
    /// Hide List에 있는 모든 UI 게임오브젝트를 삭제한다.
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
    /// 해당 UI type이 Open List에 포함되어 있나 확인한다. IsHide가 경우에 따라 더 유용하다.
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
    /// 해당 UI가 Open List에 포함되어 있나 확인한다.
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
    /// 해당 UI type이 Hide List에 포함되어 있나 확인한다.
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
    /// Hide List에 포함되어 있나 확인한다.
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
