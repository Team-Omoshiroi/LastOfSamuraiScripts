using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� �� �� ����� ��ȯ�Ѵ�. �׼� ��带 �ٽ� ��ӹ޾� ������ ����� ������ ���� ������ ����� ��������Ʈ�� �����ϵ��� �Ͽ���.
/// </summary>
public class BTAction : BTNode
{
    private System.Func<eBTNodeState> action;

    public BTAction(System.Func<eBTNodeState> act)
    {
        this.action = act;
    }

    /// <summary>
    /// Action �� �� ����� ��ȯ�Ѵ�.
    /// </summary>
    public override eBTNodeState Evaluate()
    {
        return action();
    }
}
