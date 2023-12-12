using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskDead : BTNode
{
    private NavMeshAgent agent;
    private Transform transform;
    private Animator animator;

    public TaskDead(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        animator = bt.Anime;
        agent = bt.Agent;
    }

    public override eBTNodeState Evaluate()
    {

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            bt.ResetAnimations();
            animator.SetTrigger("Dead");
        }

        //if (!animator.GetBool("Death"))
        //{
        //    bt.ResetAnimations();
        //    animator.SetBool("Death", true);
        //}

        state = eBTNodeState.Running;
        return state;
    }
}
