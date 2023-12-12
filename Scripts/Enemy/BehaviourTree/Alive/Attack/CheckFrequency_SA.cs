using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특수 공격을 지정된 주기에 기반하여 랜덤한 타이밍에 사용하도록 한다.
/// </summary>
public class CheckFrequency_SA : BTNode
{
    float frequency;

    public CheckFrequency_SA(BT bt)
    {
        frequency = bt.Frequency_SA;
    }

    public override eBTNodeState Evaluate()
    {
        //랜덤 값이 frequency 이하일 때 사용
        if(Random.Range(0,1) <= frequency)
        {
            state = eBTNodeState.Success;
            return state;
        }

        state = eBTNodeState.Failure;
        return state;
    }
}
