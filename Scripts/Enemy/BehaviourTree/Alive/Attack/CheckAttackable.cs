using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckAttackable : BTNode
{
    private NavMeshAgent agent;
    private Transform transform;
    private Animator animator;
    private float attackRange;
    private float attackPerSecond;
    private Transform target;
    private bool isWaiting = false;

    private float waitCounter = 0f;
    private float waitTime;

    /// <summary>
    /// NavMeshAgent, 공격 범위, 공격 속도에 대한 float 값이 있어야 한다.
    /// </summary>
    public CheckAttackable(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        animator = bt.Anime;
        agent = bt.Agent;
        attackRange = bt.AttackRange;
        attackPerSecond = bt.AttackPerSecond;
        waitTime = 1f / attackPerSecond;
    }

    public override eBTNodeState Evaluate()
    {
        SetIsWaiting();

        state = eBTNodeState.Failure;

        //현재 타겟이 설정되어 있다면
        if (target != null)
        {
            //타겟이 활성화 상태이거나 살아 있을 때
            if (target.gameObject.activeSelf)
            {            
                //타겟이 사망했다면 공격을 중단한다.
                if (target.TryGetComponent<HealthModule>(out HealthModule health))
                {
                    if (health.IsDead)
                    {
                        //Target 리셋, Failure.
                        target = null;
                        root.ClearData(StaticVariables.targetText);

                        state = eBTNodeState.Failure;
                        return state;
                    }
                }

                //공격 범위 내에 있다면 Success
                if (Vector3.Distance(target.transform.position, transform.position) <= attackRange && Vector3.Distance(target.transform.position, transform.position) > (attackRange * 0.8f))
                {
                    state = eBTNodeState.Success;
                }
                //타겟이 공격 범위를 벗어났다면 Failure
                else
                {
                    state = eBTNodeState.Failure;
                }
            }
            //타겟이 비활성화 상태일 때
            else
            {
                //Target 리셋, Failure.
                target = null;
                root.ClearData(StaticVariables.targetText);

                state = eBTNodeState.Failure;
            }
        }
        //현재 설정된 타겟이 존재하지 않는다면 
        else if (target == null)
        {
            //타겟을 탐색한다.
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, StaticVariables.playerLayer);

            //감지된 대상이 존재한다면 가장 처음 감지한 대상을 타겟으로 지정하고 Success.
            if (colliders.Length > 0)
            {
                root.SetData(StaticVariables.targetText, colliders[0].transform);
                target = colliders[0].transform;
            }
            //여전히 타겟을 찾을 수 없다면 Failure.
            else
            {
                state = eBTNodeState.Failure;
            }
        }

        //아직까지 Success 이고 target 이 존재한다면
        if (state == eBTNodeState.Success && target != null)
        {
            //일단 자리에 멈춰서 타겟을 바라보도록 한다.
            agent.destination = agent.transform.position;
            bt.ResetAnimations();

            Vector3 direction = target.position - transform.position; direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
        else
        {
            state = eBTNodeState.Failure;
        }

        //공격 시 콤보 갱신
        if (state == eBTNodeState.Success) 
        { 
            //첫 공격이라면 
            if(root.GetData(StaticVariables.comboText) == null)
            {
                int startCombo = Random.Range(1,StaticVariables.maxCombo-1);
                root.SetData(StaticVariables.comboText, startCombo);
            }
            else
            {
                int nextCombo = (int)root.GetData(StaticVariables.comboText) % StaticVariables.maxCombo + 1;
                root.SetData(StaticVariables.comboText, nextCombo);
            }

            if (isWaiting)
            {
                state = eBTNodeState.Failure;
            }
            else
            {
                isWaiting = true;
            }
        }
        //공격이 아예 불가능한 상태라면 콤보 초기화.
        else if (!isWaiting) 
        { 
            root.ClearData(StaticVariables.comboText); 
        }

        if(state == eBTNodeState.Success)
        {
            bt.ActivateWeapon();
        }

        return state;
    }

    private void SetIsWaiting()
    {
        //대기 상태라면 타이머를 체크한다.
        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                isWaiting = false;
                waitCounter = 0f;
            }
        }
    }
}