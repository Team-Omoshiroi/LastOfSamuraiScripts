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

        //대상과의 거리가 좁혀질 때 까지 추적한다.
        if(Vector3.Distance(transform.position, target.position) > 2f)
        {
            agent.destination = target.position;

            //실행 중인 애니메이션이 RunForward 라면 활성화하지 않고, 다른 애니메이션이 실행 중일 때만 활성화
            if (!animator.GetBool("RunForward"))
            {
                bt.ResetAnimations();
                bt.Anime.Rebind();
                animator.SetBool("RunForward", true);
            }
        }

        state = eBTNodeState.Running;
        return state;
    }
}
