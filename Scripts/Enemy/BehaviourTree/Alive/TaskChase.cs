using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskChase : BTNode
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform transform;
    private Transform temp;
    private bool isRunning = false;

    /// <summary>
    /// NavMeshAgent, Animator 가 있어야 한다.
    /// </summary>
    public TaskChase(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        agent = bt.Agent;
        animator = bt.Anime;
    }

    public override eBTNodeState Evaluate()
    {
        Transform target = (Transform)GetData(StaticVariables.targetText);

        //대상과의 거리가 공격 범위 이하가 될 때 까지 추적한다.
        if(Vector3.Distance(transform.position, target.position) >= bt.AttackRange)
        {
            agent.updateRotation = true;
            agent.destination = target.position;

            //실행 중인 애니메이션이 RunForward 라면 활성화하지 않고, 다른 애니메이션이 실행 중일 때만 활성화
            if (!animator.GetBool("RunForward"))
            {
                bt.ResetAnimations();
                bt.Anime.Rebind();
                animator.SetBool("RunForward", true);
            }
        }
        else
        {   //대상과의 거리가 너무 가깝다면 뒷걸음질 치며 일정 거리를 유지한다.
            if (Vector3.Distance(transform.position, target.position) < bt.AttackRange * 0.7f)
            {
                agent.updateRotation = false;

                Vector3 direction = target.position - agent.transform.position;


                agent.destination = new Vector3(-target.position.x, -target.position.y, -target.position.z);

                //실행 중인 애니메이션이 WalkBackward 라면 활성화하지 않고, 다른 애니메이션이 실행 중일 때만 활성화
                if (!animator.GetBool("WalkBackward"))
                {
                    bt.ResetAnimations();
                    bt.Anime.Rebind();
                    animator.SetBool("WalkBackward", true);
                }
            }
        }

        state = eBTNodeState.Running;
        return state;
    }
}
