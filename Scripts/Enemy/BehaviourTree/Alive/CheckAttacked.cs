using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



/// <summary>
/// 공격받았는지의 여부를 체크한다. 아직 추적 상태가 아니라면 즉시 추적 상태가 된다. 인지 범위 내에서 일정 시간 머무를 경우 추격한다.
/// 한 번 인식한 대상이 사라질 때 까지 추격하도록 한다.
/// </summary>
public class CheckAttacked : BTNode
{
    private NavMeshAgent agent;
    private Transform transform;
    private Animator animator;
    private Transform hitInfo;
    private BoxCollider collider;
    private float groggyTime;
    private float groggyCounter = 0f;
    private bool isGroggy = false;
    private float hitDelayTime;
    private float hitDelayCounter = 0f;
    private bool isHitDelay = false;

    /// <summary>
    /// 인지 범위에 대한 float 값이 있어야 한다.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="fovRange"></param>
    public CheckAttacked(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        collider = transform.GetComponent<BoxCollider>();
        animator = bt.Anime;
        agent = bt.Agent;
        groggyTime = bt.GroggyTime;
        hitDelayTime = bt.HitDelayTime;
    }

    public override eBTNodeState Evaluate()
    {
        //먼저 공격 판정에 대한 Collider 와의 충돌을 감지한다.
        //충돌 감지 시 피격 처리와 피격 애니메이션을 재생하며 일정 시간 동안 그로기 상태가 된다. 그로기 상태에서는 Runnning 을 반환하여 아무런 행동을 할 수 없게 한다.

        //피격 처리 또한 짧은 대기 시간을 가진다.
        //그로기 상태인지의 여부를 체크한다. 이미 그로기 상태라면 지속시간이 더 늘어나지는 않으며, 피격 처리와 피격 애니메이션 재생만 이루어진다.
        //그로기 상태는 일정 시간 뒤 해제된다.

        //피격 여부 체크와 그로기 상태 체크는 개별적으로 이루어진다.

        //플레이어의 공격 판정을 가진 무언가를 감지한다
        state = eBTNodeState.Failure;

        SetHitDelay();
        SetGroggy();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, agent.radius, StaticVariables.playerAttackLayer);

        //감지된 공격이 존재한다면
        if (colliders.Length > 0)
        {
            //피격 처리 불가 상태가 아니라면
            if (!isHitDelay)
            {
                isHitDelay = true;
                hitDelayCounter = 0;

                //피격 방향에 대한 데이터와 피해량 데이터를 세팅하는 내용

                hitInfo = colliders[0].transform;
                root.SetData(StaticVariables.hitInfoText, hitInfo);

                Debug.Log("Player attack detected!");

                //그로기 상태가 아니라면 그로기 상태를 활성화한다.
                if (!isGroggy)
                {
                    isGroggy = true;
                    groggyCounter = 0;
                }

                SetIsAttackedData(true);
                state = eBTNodeState.Success;
                return state;
            }
            else
            {
                Debug.Log("Player attack Not detected!");
                SetIsAttackedData(false);
                state = eBTNodeState.Success;
                return state;
            }
        }
        //감지된 공격이 존재하지 않는다면
        else
        {
            SetIsAttackedData(false);

            //그로기 상태일 때에는 추가적인 행동을 할 수 없도록 Success.
            if (isGroggy)
            {
                state = eBTNodeState.Success;
            }
            else
            {
                state = eBTNodeState.Failure;
            }
        }

        return state;
    }

    private void SetGroggy()
    {
        //그로기 상태라면
        if (isGroggy)
        {
            //그로기 카운터 증가
            groggyCounter += Time.deltaTime;

            //그로기 시간이 다 되었다면 그로기 상태를 비활성화한다.
            if (groggyCounter >= groggyTime)
            {
                isGroggy = false;
                groggyCounter = 0;
            }
        }
    }

    private void SetHitDelay()
    {
        //피격 상태라면
        if (isHitDelay)
        {
            //피격 딜레이 카운터 증가
            hitDelayCounter += Time.deltaTime;

            //피격 딜레이 시간이 다 되었다면 피격 상태를 비활성화한다.
            if (hitDelayCounter >= hitDelayTime)
            {
                isHitDelay = false;
                hitDelayCounter = 0;
            }
        }
    }

    private void SetIsAttackedData(bool isAttacked)
    {
        root.SetData(StaticVariables.isAttackedText, isAttacked);
    }
}
