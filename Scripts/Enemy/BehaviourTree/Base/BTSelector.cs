using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 처음으로 자식 노드가 성공했다면 중단 후 Success 반환, 모든 자식 노드가 실패했을 때에만 Failure 를 반환한다. (OR 와 같다)
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
