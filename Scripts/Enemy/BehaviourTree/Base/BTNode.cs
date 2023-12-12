using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode
{
    protected eBTNodeState state;
    protected BT bt;
    protected BTNode root;   
    protected BTNode parent;
    protected List<BTNode> children = new List<BTNode>();
    private Dictionary<string, object> dataContext = new Dictionary<string, object>();

    public BTNode()
    {
        parent = null;
    }

    public BTNode(List<BTNode> nodeList)
    {
        foreach (BTNode node in nodeList)
        {
            AddChild(node);
        }
    }

    public void AddChild(BTNode node)
    {
        node.parent = this; children.Add(node);
    }

    /// <summary>
    /// 현재 노드의 상태(Success, Failure, Running) 를 반환한다.
    /// </summary>
    /// <returns></returns>
    public virtual eBTNodeState Evaluate()
    {
        return eBTNodeState.Failure;
    }

    public void SetData(string key, object value)
    {
        dataContext[key] = value;
    }

    /// <summary>
    /// 현재 노드에서 값을 찾을 수 없다면 부모 노드 방향으로 이동하며 key 에 대응되는 value 를 탐색한다.
    /// 현재 노드~부모 노드에서도 값을 찾을 수 없다면 null 을 반환한다.
    /// </summary>
    public object GetData(string key)
    {
        object value = null;

        if (dataContext.TryGetValue(key, out value)) { return value; }

        BTNode node = parent;

        while (node != null)
        {
            value = node.GetData(key);

            if(value != null)
            {
                return value;
            }

            node = node.parent;
        }

        return null;
    }

    /// <summary>
    /// 현재 노드에서 값을 찾을 수 없다면 부모 노드 방향으로 이동하며 key 에 대응되는 value 를 찾아 제거 후 true 를 반환한다.
    /// 현재 노드~부모 노드에서도 값을 찾을 수 없다면 false 을 반환한다.
    /// </summary>
    public bool ClearData(string key)
    {
        if (dataContext.ContainsKey(key))
        {
            dataContext.Remove(key);
            return true;
        }

        BTNode node = parent;

        while (node != null)
        {
            if (node.dataContext.ContainsKey(key))
            {
                node.dataContext.Remove(key);
                return true;
            }

            node = node.parent;
        }

        return false;
    }
}
