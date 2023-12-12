using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특수 공격이 재사용 대기 시간인지 검사한다.
/// </summary>
public class CheckCoolDown_SA : BTNode
{
    private bool isCoolDown = true;

    private float coolDownCounter = 0f;
    private float coolDownTime;

    public CheckCoolDown_SA(BT bt)
    {
        root = bt.Root;
        coolDownTime = bt.CoolDownTime_SA;
    }

    public override eBTNodeState Evaluate()
    {
        SetIsCoolDown();

        if (isCoolDown)
        {
            state = eBTNodeState.Success;
            return state;
        }
        else
        {
            state = eBTNodeState.Failure;
            return state;
        }
    }

    private void SetIsCoolDown()
    {
        if (root.GetData(StaticVariables.saCooldownText) != null)
        {
            isCoolDown = (bool)root.GetData(StaticVariables.saCooldownText);
            coolDownCounter = isCoolDown ? 0 : coolDownCounter;
            root.ClearData(StaticVariables.saCooldownText);
        }

        if (!isCoolDown)
        {
            coolDownCounter += Time.deltaTime;
            if (coolDownCounter >= coolDownTime)
            {
                isCoolDown = true;
                coolDownCounter = 0;
            }
        }
    }
}
