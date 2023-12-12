using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCondition : BTNode
{
    private System.Func<bool> condition;

    public BTCondition(System.Func<bool> con)
    {
        condition = con;
    }

    public override eBTNodeState Evaluate()
    {
        return condition() ? eBTNodeState.Success : eBTNodeState.Failure;
    }
}
