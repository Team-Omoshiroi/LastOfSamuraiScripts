using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ó������ �ڽ� ��尡 �����ߴٸ� �ߴ� �� Success ��ȯ, ��� �ڽ� ��尡 �������� ������ Failure �� ��ȯ�Ѵ�. (OR �� ����)
/// </summary>
public class BTSelector : BTNode
{

    public override eBTNodeState Evaluate()
    {

        foreach (BTNode node in children)
        {
            eBTNodeState result = node.Evaluate();
            if (result == eBTNodeState.Success)
            {
                return eBTNodeState.Success;
            }
            else if (result == eBTNodeState.Running)
            {
                return eBTNodeState.Running;
            }
        }

        return eBTNodeState.Failure;
    }
}
