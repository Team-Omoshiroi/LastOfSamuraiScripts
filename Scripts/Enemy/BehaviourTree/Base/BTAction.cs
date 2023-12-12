using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 동작 수행 후 평가 결과를 반환한다. 액션 노드를 다시 상속받아 별도로 기능을 구현할 수도 있으나 현재는 델리게이트로 수행하도록 하였다.
/// </summary>
public class BTAction : BTNode
{
    private System.Func<eBTNodeState> action;

    public BTAction(System.Func<eBTNodeState> act)
    {
        this.action = act;
    }

    /// <summary>
    /// Action 의 평가 결과를 반환한다.
    /// </summary>
    public override eBTNodeState Evaluate()
    {
        return action();
    }
}
