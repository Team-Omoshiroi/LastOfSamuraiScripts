using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 자식이 성공이라면 Success 반환, 하나라도 실패했다면 중단 후 Failure 반환, 아직 실행이 덜 된 자식이 있다면 Running 반환
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
