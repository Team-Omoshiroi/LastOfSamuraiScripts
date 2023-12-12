using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskAttacked : BTNode
{
    private Animator animator;
    private NavMeshAgent agent;
    private Transform transform;
    private float attackRange;

    /// <summary>
    /// 공격 범위, 공격 속도에 대한 float 값이 있어야 한다.
    /// </summary>
    public TaskAttacked(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        animator = bt.Anime;
        agent = bt.Agent;
        attackRange = bt.AttackRange;
    }

    public override eBTNodeState Evaluate()
    {
        Transform hitInfo = (Transform)root.GetData(StaticVariables.hitInfoText);
        bool isAttacked = (bool)root.GetData(StaticVariables.isAttackedText);

        //Debug.Log($"isAttacked : {isAttacked}");

        agent.destination = agent.transform.position;

        if (isAttacked)
        {
            //추후, 피격 방향에 따른 애니메이션 구분 예정

            AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);

            //피격에 대한 실질적인 처리가 이루어지는 부분
            Debug.Log("Attacked!");
            bt.ResetAnimations();
            //animator.SetBool("HitFront", true);
            animator.SetTrigger("GetHitFront");
        }

        state = eBTNodeState.Running;
        return state;
    }
}
