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
    /// ���� ����� ����(Success, Failure, Running) �� ��ȯ�Ѵ�.
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
    /// ���� ��忡�� ���� ã�� �� ���ٸ� �θ� ��� �������� �̵��ϸ� key �� �����Ǵ� value �� Ž���Ѵ�.
    /// ���� ���~�θ� ��忡���� ���� ã�� �� ���ٸ� null �� ��ȯ�Ѵ�.
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
    /// ���� ��忡�� ���� ã�� �� ���ٸ� �θ� ��� �������� �̵��ϸ� key �� �����Ǵ� value �� ã�� ���� �� true �� ��ȯ�Ѵ�.
    /// ���� ���~�θ� ��忡���� ���� ã�� �� ���ٸ� false �� ��ȯ�Ѵ�.
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
