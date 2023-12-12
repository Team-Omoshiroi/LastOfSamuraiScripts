using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckArrived : BTNode
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform transform;
    private Transform[] waypoints;

    /// <summary>
    /// NavMeshAgent, Animator 가 있어야 한다.
    /// </summary>
    public CheckArrived(Transform transform, Transform[] waypoints)
    {
        this.transform = transform;
        agent = transform.GetComponent<NavMeshAgent>();
        animator = transform.GetComponent<Animator>();

        this.waypoints = waypoints;
    }

    public override eBTNodeState Evaluate()
    {
        return base.Evaluate();
    }
}
