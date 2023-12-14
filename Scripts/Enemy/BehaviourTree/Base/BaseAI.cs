using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class BaseAI : BT
{
    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        //피격 등의 사유로 인한 일시적인 행동 제한을 위함
        SetInterrupt();

        SetUnstoppable();

        if (!isGroggy && !isParrying)
        {
            base.Update();
        }
    }

    protected override void SetupTree()
    {
        Root = new BTSelector();

        SelDead selDead = new SelDead(this);
        SelAlive selAlive = new SelAlive(this);

        Root.AddChild(selDead); Root.AddChild(selAlive);
    }


    //플레이어의 공격 판정을 가진 무언가를 감지했을 때, 피격 메소드를 호출한다.
    //'isDefensible' 상태일 때 플레이어의 방어 판정을 가진 무언가를 감지했다면 '방어당함' 메소드를 호출한다.
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("PlayerAttack"))
        {
            Attacked(other);
        }
        else if (IsDefensible && other.CompareTag("PlayerDefense"))
        {
            Blocked(other);
        }
    }

    /// <summary>
    /// 피격 반응에 대한 메소드. 조건이 맞아떨어진다면 패링으로 대응하도록 한다.
    /// </summary>
    private void Attacked(Collider hitInfo)
    {
        if (CheckAttacked(hitInfo))
        {
            //if (CheckParryable(hitInfo))
            //{
            //    TaskParry(hitInfo);
            //    return;
            //}

            TaskAttacked(hitInfo);
        }
    }

    /// <summary>
    /// 피격 처리가 가능한 상태인지 확인하는 메소드
    /// </summary>
    private bool CheckAttacked(Collider hitInfo)
    {
        SetInterrupt();

        //피격 처리 불가 상태가 아니라면
        if (!isHitDelay)
        {
            Debug.Log("Player attack check success!");
            return true;
        }
        else
        {
            Debug.Log("Player attack check failed!");
            return false;
        }
    }

    //실질적인 피격 처리를 담당하는 메소드
    private void TaskAttacked(Collider hitInfo)
    {
        //피격 처리와 피격 애니메이션을 재생하며 일정 시간 동안 그로기 상태가 된다. 그로기 상태에서는 아무런 행동을 할 수 없게 한다.

        //피격 처리 또한 짧은 대기 시간을 가진다.

        //그로기 상태인지의 여부를 체크한다. 이미 그로기 상태라면 지속시간이 더 늘어나지는 않으며, 피격 처리와 피격 애니메이션 재생만 이루어진다.
        //그로기 상태는 일정 시간 뒤 해제된다.

        //피격 여부 체크와 그로기 상태 체크는 개별적으로 이루어진다.

        Agent.destination = Agent.transform.position;

        //추후, 피격 방향에 따른 애니메이션 구분 예정

        //피격에 대한 실질적인 처리가 이루어지는 부분
        Debug.Log("Attacked!");

        isHitDelay = true;
        hitDelayCounter = 0;

        //저지 불가 상태가 아니라면 피격에 대한 반응까지 처리한다.
        if (!isUnstoppable)
        {
            //그로기 상태가 아니라면 그로기 상태를 활성화한다.
            if (!isGroggy)
            {
                isGroggy = true;
                groggyCounter = 0;
            }

            ResetAnimations();
            Anime.Rebind();
            Anime.SetTrigger("GetHitFront");
        }
    }

    private bool CheckParryable(Collider hitInfo)
    {
        //그로기 상태가 아니면서 공격 중 허점을 드러낸 상태가 아닐 때(저지 불가 상태 제외), 또는 패링 상태일 때 지정된 확률로 공격을 패링한다.
        if (!isGroggy && !isDefensible && !isUnstoppable || isParrying)
        {
            Debug.Log("Parryable check success!");
            if (Random.Range(0, 1) <= Frequency_parrying)
            {
                return true;
            }
        }

        Debug.Log("Parryable check failed!");
        return false;
    }

    private void TaskParry(Collider hitInfo)
    {
        Agent.destination = Agent.transform.position;

        //추후, 패링 방향에 따른 애니메이션 구분 예정

        //패링에 대한 실질적인 처리가 이루어지는 부분
        Debug.Log("Parrying!");

        isHitDelay = true;
        hitDelayCounter = 0;

        IsParrying = true;
        parryingCounter = 0;

        ResetAnimations();
        Anime.Rebind();
        Anime.SetTrigger("Parrying");
    }

    /// <summary>
    /// '방어당함' 처리 메소드
    /// </summary>
    private void Blocked(Collider blockInfo)
    {
        if (CheckBlocked(blockInfo))
        {
            TaskBlocked(blockInfo);
        }
    }

    /// <summary>
    /// 공격이 막힌 상태의 처리가 가능한지 확인하는 메소드
    /// </summary>
    private bool CheckBlocked(Collider hitInfo)
    {
        //블록 처리와 블록 애니메이션을 재생하며 일정 시간 동안 그로기 상태가 된다. 그로기 상태에서는 아무런 행동을 할 수 없게 한다.

        //블록 처리 또한 짧은 대기 시간을 가진다.

        //그로기 상태인지의 여부를 체크한다. 이 경우에는 그로기 상태의 지속시간이 갱신된다.
        //그로기 상태는 일정 시간 뒤 해제된다.

        //피격 여부 체크와 그로기 상태 체크는 개별적으로 이루어진다.

        SetInterrupt();

        //피격 처리 불가 상태가 아니라면
        if (!isHitDelay)
        {
            isHitDelay = true;
            hitDelayCounter = 0;

            Debug.Log("Blocked check success!");

            //그로기 상태를 갱신한다.
            isGroggy = true;
            groggyCounter = 0;

            return true;
        }
        else
        {
            Debug.Log("Blocked check failed!");
            return false;
        }
    }

    /// <summary>
    /// 실질적인 '방어당함' 처리를 담당하는 메소드
    /// </summary>
    private void TaskBlocked(Collider hitInfo)
    {
        Agent.destination = Agent.transform.position;

        //추후, 방향에 따른 애니메이션 구분 예정

        //'방어당함'에 대한 실질적인 처리가 이루어지는 부분
        
        Debug.Log("Blocked!");
        ResetAnimations();
        Anime.SetTrigger("Block");

        Agent.destination = Agent.transform.position;
    }

    /// <summary>
    /// 피격 등의 여파로 발생한 직접적인 행동 방해 효과의 지속 시간을 다룬다.
    /// </summary>
    private void SetInterrupt()
    {
        float time = Time.deltaTime;

        //그로기 상태라면
        if (isGroggy)
        {
            //그로기 카운터 증가
            groggyCounter += time;

            //그로기 시간이 다 되었다면 그로기 상태를 비활성화한다.
            if (groggyCounter >= GroggyTime)
            {
                isGroggy = false;
                groggyCounter = 0;
            }
        }

        //피격 상태라면
        if (isHitDelay)
        {
            //피격 딜레이 카운터 증가
            hitDelayCounter += time;

            //피격 딜레이 시간이 다 되었다면 피격 상태를 비활성화한다.
            if (hitDelayCounter >= HitDelayTime)
            {
                isHitDelay = false;
                hitDelayCounter = 0;
            }
        }

        //'방어당함' 상태가 될 수 있는 상태라면
        if (isDefensible)
        {
            //카운터 증가
            defensibleCounter += time;

            //시간이 다 되었다면 현재 상태를 비활성화한다.
            if (defensibleCounter >= DefensibleTime)
            {
                isDefensible = false;
                defensibleCounter = 0;
            }
        }

        //'패링' 상태라면
        if (IsParrying)
        {
            parryingCounter += time;

            if (parryingCounter >= ParryingTime)
            {
                isParrying = false;
                parryingCounter = 0;
            }
        }
    }

    /// <summary>
    /// 저지 불가 상태의 지속 시간을 다룬다.
    /// </summary>
    private void SetUnstoppable()
    {
        if (isUnstoppable)
        {
            unstoppableCounter += Time.deltaTime;

            if(unstoppableCounter >= UnstoppableTime)
            {
                isUnstoppable = false;
                unstoppableCounter = 0;
            }
        }
    }
}


