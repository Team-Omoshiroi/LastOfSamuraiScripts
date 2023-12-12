using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 지정된 위치들을 순서대로 이동한다. 지정된 위치에 도달할 때 마다 잠시 대기했다가 다음 위치로 이동한다. 
/// </summary>
public class TaskPatrol : BTNode
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform transform;
    private Transform[] waypoints;

    private int currentWaypointIndex = 0;
    private Vector3 currentDestination;
    private Vector3 temp;

    private float waitTime;
    private float waitCounter = 0f;
    private bool waiting = true;

    /// <summary>
    /// NavMeshAgent, Animator 가 있어야 한다.
    /// </summary>
    public TaskPatrol(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        agent = bt.Agent;
        animator = bt.Anime;
        waypoints = bt.Waypoints;
        waitTime = bt.PatrolWaitTime;
    }

    public override eBTNodeState Evaluate()
    {
        //대기 상태라면 대기 시간이 지날 때 까지 대기 상태를 유지한다.
        if (waiting)
        {
            waitCounter += Time.deltaTime;

            //대기 상태가 해제된 직후 agent 의 다음 목적지를 지정해준다.
            if (waitCounter >= waitTime)
            {
                waitCounter = 0f;
                currentDestination = waypoints[currentWaypointIndex].position;
                agent.destination = currentDestination;
                bt.ResetAnimations();
                bt.Anime.Rebind();
                animator.SetBool("WalkForward", true);
                waiting = false;
            }
        }
        else
        {
            //목적지에 매우 근접했다면 현재 위치로 고정시키고, 배열의 다음 인덱스에 해당하는 목적지를 설정하고 대기 상태로 만든다.
            if (Vector3.Distance(transform.position, currentDestination) < 2f)
            {
                agent.destination = agent.transform.position;
                waitCounter = 0f;
                waiting = true;
                animator.SetBool("WalkForward", false);

                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            //추적 등의 사유로 경로를 벗어났다가 다시 순찰 상태로 복귀한 경우에도 다시 agent 의 목적지를 원래대로 바꾸어 주어야 한다.
            else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WalkForward"))
            {
                agent.destination = currentDestination;
                bt.ResetAnimations();
                animator.SetBool("WalkForward", true);
            }
        }

        state = eBTNodeState.Running;
        return state;
    }
}
