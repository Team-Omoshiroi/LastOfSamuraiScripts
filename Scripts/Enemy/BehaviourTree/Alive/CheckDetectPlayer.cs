using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 범위 내에 존재하는 플레이어가 있는지의 여부를 체크한다. 플레이어가 인지 범위 내에서 일정 시간 머무를 경우 추격한다.
/// 한 번 인식한 대상이 사라질 때 까지 추격하도록 한다.
/// </summary>
public class CheckDetectPlayer : BTNode
{
    private NavMeshAgent agent;
    private Transform transform;
    private Animator animator;
    private float fovRange;
    private Transform target;
    private float WaitngTime;

    private float recognitionTime;
    private float recognitionCounter = 0f;
    private bool isChasing = false;
    private bool isRunning = false;

    /// <summary>
    /// 인지 범위에 대한 float 값이 있어야 한다.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="fovRange"></param>
    public CheckDetectPlayer(BT bt)
    {
        this.bt = bt;
        root = bt.Root;
        transform = bt.transform;
        animator = bt.Anime;
        fovRange = bt.FovRange;
        recognitionTime = bt.RecognitionTime;
        agent = bt.Agent;
    }

    public override eBTNodeState Evaluate()
    {
        //현재 설정된 타겟이 존재한다면 
        if(target != null)
        {
            //타겟이 활성화 된 상태라면
            if (target.gameObject.activeSelf)
            {
                //추격 트리거가 활성화되었다면 Success.
                if (isChasing)
                {
                    root.SetData(StaticVariables.targetText, target);
                    state = eBTNodeState.Success;
                }
                //추격 상태가 아니고 인지 범위 내에 있다면
                else if (!isChasing && Vector3.Distance(target.transform.position, transform.position) <= fovRange)
                {
                    //타겟이 매우 가까운 위치에 있다면 즉시 추격 트리거를 활성화하고 인지 카운터를 초기화한다.
                    if (Vector3.Distance(target.transform.position, transform.position) <= (fovRange * 0.7))
                    {
                        isChasing = true;
                        recognitionCounter = 0;
                        state = eBTNodeState.Success;
                    }
                    //아니라면 인지 카운터를 증가시킨다.
                    else
                    {
                        recognitionCounter += Time.deltaTime;

                        //인지 시간동안 플레이어가 감지 범위 내에서 머물렀다면 추격 트리거를 활성화하고 Success.
                        if (recognitionCounter >= recognitionTime)
                        {
                            isChasing = true;
                            recognitionCounter = 0;
                            state = eBTNodeState.Success;
                        }
                        //아직 인지 시간에 도달하지 않았다면 Running.
                        else
                        {
                            state = eBTNodeState.Running;
                        }
                    }
                }
                //추격 상태가 아니고 인지 범위를 벗어났다면
                else if (!isChasing && Vector3.Distance(target.transform.position, transform.position) > fovRange)
                {
                    //Target 해제, 인지 카운터가 감소하고 Failure.
                    target = null;

                    recognitionCounter = recognitionCounter < 0 ? recognitionCounter -= Time.deltaTime : 0;

                    state = eBTNodeState.Failure;
                }
            }
            //타겟이 비활성화 된 상태라면
            else
            {
                //타겟 정보를 초기화하고 Failure.
                isChasing = false;
                target = null;
                root.ClearData(StaticVariables.targetText);
                state = eBTNodeState.Failure;
            }
        }
        //현재 설정된 타겟이 존재하지 않는다면 
        else if (target == null)
        {
            //먼저, 기존의 타겟 정보를 탐색한다.
            target = (Transform)root.GetData(StaticVariables.targetText);

            //여전히 타겟 정보가 없다면
            if(target == null)
            {
                //타겟을 탐색한다.
                Collider[] colliders = Physics.OverlapSphere(transform.position, fovRange, StaticVariables.playerLayer);

                //감지된 대상이 존재한다면 가장 처음 감지한 대상을 타겟으로 지정하고 Success.
                if (colliders.Length > 0)
                {
                    root.SetData(StaticVariables.targetText, colliders[0].transform);
                    state = eBTNodeState.Success;
                }
                //아직까지도 타겟을 찾을 수 없다면 Failure.
                else
                {
                    isChasing = false;
                    state = eBTNodeState.Failure;
                }
            }
        }

        return state;
    }
}
