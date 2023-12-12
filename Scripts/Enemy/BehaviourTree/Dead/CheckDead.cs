using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckDead : BTNode
{
    private Transform transform;

    public CheckDead(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
    }

    public override eBTNodeState Evaluate()
    {
        if (bt.IsDead)
        {
            state = eBTNodeState.Success;
        }
        else
        {
            state = eBTNodeState.Failure;
        }

        return state;
    }
}
