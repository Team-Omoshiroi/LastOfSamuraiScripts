using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �ڽ��� �����̶�� Success ��ȯ, �ϳ��� �����ߴٸ� �ߴ� �� Failure ��ȯ, ���� ������ �� �� �ڽ��� �ִٸ� Running ��ȯ
/// </summary>
/// <returns></returns>
public class BTSequence : BTNode
{
    public override eBTNodeState Evaluate()
    {
        bool isAnyChildRunning = false;

        foreach (BTNode node in children)
        {
            eBTNodeState result = node.Evaluate();
            if(result == eBTNodeState.Failure)
            {
                return eBTNodeState.Failure;
            }
            else if (result == eBTNodeState.Running)
            {
                isAnyChildRunning = true;
            }
        }

        return isAnyChildRunning ? eBTNodeState.Running : eBTNodeState.Success;
    }
}
