using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//[최우선 구현]
//1.대기 상태: 배회 상태에서 현재 목표 지점까지 도달했을 때 랜덤한 시간 동안 아무 것도 하지 않고 제자리에 머무른다. 
//별다른 트리거 없이 지정된 시간이 경과하면 배회 상태로 진입한다.
//(base 의 통상 상태를 유지한다)

//[최우선 구현]
//2.배회 상태: 별다른 트리거가 없다면, 지정된 목표 지점들 중 하나를 선택하여 해당 위치까지 이동한다. 
//목표 지점에 도달했다면 대기 상태로 진입한다.
//(base 의 걷기 상태를 유지한다)

//[최우선 구현]
//3.추적 상태: 적대 대상을 확실하게 감지했을 때(일단은 일정 범위 내에 들어왔을 때 한정) 이 상태가 된다. 적대 대상의 위치를 목표 지점으로 지정하고,
//지속적으로 목표 지점을 갱신하며 접근한다. 적대 대상이 공격 범위 내에 들어왔다면 공격을 시도한다.
//(적대 대상에게 다가갈 때는 base 의 달리기 상태를 유지한다.)
//(적대 대상과 가까워졌을 때는 base 의 조준 - 공격 상태를 유지한다. 거리가 멀어지면 이를 반복한다)

//[추후 검토]
//4.수색 상태: 공격받았지만 아직 적대 대상을 확실하게 감지하지 못 했을 때(일단은 일정 범위 내에 들어왔을 때 한정), 
//추적 상태에서 적대 대상과 거리가 멀어졌을 때 이 상태가 된다. 감지 범위가 크게 증가하며,
//현재 위치 내에서 일정 범위를 배회한다. 이 상태에서 일정 시간 별다른 트리거가 없었다면 배회 상태로 진입한다.
//(base 의 걷기 상태를 유지한다)

//[추후 검토]
//5.도망 상태: 체력이 낮을 때 적대 대상을 확실하게 감지했다면, 일정 확률로 이 상태가 된다. 도망 상태는 일정 시간 유지되며, 적대 대상과 멀어지는 방향으로 이동한다. 
//이 상태가 유지되는 동안 대부분의 트리거를 무시한다. 
//(base 의 달리기 상태를 유지한다)



public class EnemyAIController : MonoBehaviour
{
    [SerializeField] private EnemyDataContainer dataContainer;
    
    //길찾기용 컴포넌트
    [SerializeField] private NavMeshAgent agent;

    //목적지 좌표
    [SerializeField] private List<Vector3> wanderDestinations;

    [SerializeField] private float characterRadiusMultiplier = 1.5f;
    [SerializeField] private float minWanderDistance = 5f;
    [SerializeField] private float maxWanderDistance = 15f;
    [SerializeField] private float normDetectRadius = 4f;
    [SerializeField] private float maxDetectRadius = 8f;
    [SerializeField] private float chaseDistance = 15f;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float searchTime = 3f;
    [SerializeField] private float runawayTime = 3f;


    private Vector2 viewDirection = new Vector2();
    private Vector2 aimDirection = new Vector2();
    private Vector3 curDestination;
    private GameObject target;
    private SphereCollider collider;

    private float characterRadius;

    private eStateType state;
    private eAIStateType aiState;

    private bool isRun = false;
    private float moveSpeed = 4f;

    public List<Vector3> WanderDestinations { get { return wanderDestinations; } private set { wanderDestinations = value; } }
    public NavMeshAgent Agent { get { return agent; } private set { agent = value; } }
    public float MinWanderDistance { get { return minWanderDistance; } private set { minWanderDistance = value; } }
    public float MaxWanderDistance { get { return maxWanderDistance; } private set { maxWanderDistance = value; } }
    public float NormDetectRadius { get { return normDetectRadius; } private set { normDetectRadius = value; } }
    public float MaxDetectRadius { get { return MaxDetectRadius; } private set { MaxDetectRadius = value; } }
    public float ChaseDistance { get { return chaseDistance; } private set { chaseDistance = value; } }
    public float AttackDistance { get { return attackDistance; } private set { attackDistance = value; } }

    public Vector2 ViewDirection { get { return viewDirection; } private set { viewDirection = value; } }
    public Vector2 AimDirection { get { return aimDirection; } private set { aimDirection = value; } }
    public Vector3 CurDestination { get { return curDestination; } private set { curDestination = value; } }

    public GameObject Target { get { return target; } private set { target = value; } }

    public eStateType State { get { return state; } private set { state = value; } }
    public eAIStateType AIState { get { return aiState; } private set { aiState = value; } }


    private eAIStateType aiStateTemp;

    private void Awake()
    {
        characterRadius = agent.radius * characterRadiusMultiplier;
        State = eStateType.Idle;
        AIState = eAIStateType.Wait;
        //Agent.updateRotation = false;
        collider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        //csm = dataContainer.StateMachine;
        //input = dataContainer.InputActions;
        //stats = dataContainer.Stats;
        //healthSystem = dataContainer.Health;
        //moveSpeed = stats.MoveSpeed;
        //collider.radius = normDetectRadius;
        //healthSystem.OnDamaged += () => SetSearchState();
        //healthSystem.OnHealed += () => SetSearchState();
    }

    private void Update()
    {
        UpdateAIState();
    }

    public void UpdateAIState()
    {
        if (aiState != eAIStateType.Chase) { isRun = false; }
        if (aiState != eAIStateType.Search) { collider.radius = normDetectRadius; }
        if (aiState != aiStateTemp) { Debug.Log($"AIState Change : {aiStateTemp} => {aiState}"); }
        aiStateTemp = aiState;

        switch (aiState)
        {
            case eAIStateType.Idle: { WaitState(); break; }
            case eAIStateType.Wait: { WaitState(); break; }
            case eAIStateType.Wander: { WanderState(); break; }
            case eAIStateType.Chase: { ChaseState(); break; }
            case eAIStateType.Attack: { AttackState(); break; }
            case eAIStateType.Search: { SearchState(); break; }
            default: { break; }
        }
    }

    /// <summary>
    /// 대기 상태: 배회 상태에서 현재 목표 지점까지 도달했을 때 랜덤한 시간 동안 아무 것도 하지 않고 제자리에 머무른다. 
    /// 별다른 트리거 없이 지정된 시간이 경과하면 배회 상태로 진입한다.(base 의 통상 상태를 유지한다)
    /// </summary>
    private void WaitState()
    {
        if(aiState == eAIStateType.Wait)
        {
            //멈춘 상태에서의 애니메이션 재생
            //input.CallMoveEvent(Vector2.zero);
            Invoke(nameof(SetWanderState), 1f);
            aiState = eAIStateType.Idle;
        }
    }

    /// <summary>
    /// 배회 상태: 별다른 트리거가 없다면, 지정된 목표 지점들 중 하나를 선택하여 해당 위치까지 이동한다. 
    /// 목표 지점에 도달했다면 대기 상태로 진입한다.(base 의 걷기 상태를 유지한다)
    /// </summary>
    private void WanderState()
    {
        viewDirection = new Vector2(agent.velocity.x, agent.velocity.z);
        //input.CallMoveEvent(viewDirection);

        //목표 지점에 도달했다면 대기 상태로 전환
        if (IsArrived())
        {
            SetWaitState();
        }
    }

    /// <summary>
    /// 추적 상태: 적대 대상을 확실하게 감지했을 때(일단은 일정 범위 내에 들어왔을 때 한정) 이 상태가 된다. 적대 대상의 위치를 목표 지점으로 지정하고,
    /// 지속적으로 목표 지점을 갱신하며 접근한다. 적대 대상이 공격 범위 내에 들어왔다면 공격 상태로 전환한다. 
    /// (적대 대상에게 다가갈 때는 base 의 달리기 상태를 유지한다.)
    /// </summary>
    private void ChaseState()
    {
        SetNextDestination(target.transform.position);

        viewDirection = new Vector2(agent.velocity.x, agent.velocity.z);
        aimDirection = new Vector2(agent.velocity.x, agent.velocity.z);

        //input.CallRunEvent(isRun);

        if (!isRun) { isRun = true; }

        //추적 대상이 추적 범위를 벗어났다면 배회 상태로 전환
        if (Vector3.Distance(transform.position, curDestination) > chaseDistance)
        {
            SetWanderState();
            ChangeBGM();
            return;
        }

        //추적 대상이 공격 범위 안쪽으로 들어왔다면 공격 상태로 전환
        if (Vector3.Distance(transform.position, curDestination) < attackDistance)
        {
            SetAttackState(target);
            return;
        }
    }

    /// <summary>
    /// 공격 상태: 적대 대상에게 공격을 시도한다.
    /// (적대 대상 방향으로 base 의 조준 - 공격 상태를 유지한다. 대상이 공격 범위를 벗어나면 추적 상태로 전환한다)
    /// </summary>
    private void AttackState()
    {
        agent.isStopped = true;

        viewDirection = new Vector2(agent.velocity.x, agent.velocity.z);
        aimDirection = new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.z - this.transform.position.z);
        //input.CallAimEvent(aimDirection);
        Invoke(nameof(DoAttack), attackDelay);

        //추적 대상이 공격 범위를 벗어났다면 추적 상태로 전환
        if (Vector3.Distance(transform.position, curDestination) > attackDistance)
        {
            agent.isStopped = false;
            SetChaseState(target);
            return;
        }
    }

    /// <summary>
    /// 수색 상태 : 공격받았지만 아직 적대 대상을 확실하게 감지하지 못 했을 때(일정 범위 내에 적대 대상이 없는 경우), 
    /// 또는 추적 상태에서 적대 대상과 거리가 멀어졌을 때 이 상태가 된다. 감지 범위가 크게 증가하며, 
    /// 현재 위치 내에서 일정 범위를 배회한다. 이 상태에서 일정 시간 별다른 트리거가 없었다면 배회 상태로 진입한다.(base 의 걷기 상태를 유지한다) 
    /// </summary>
    private void SearchState()
    {
        viewDirection = new Vector2(agent.velocity.x, agent.velocity.z);

        if (target != null) { aimDirection = new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.z - this.transform.position.z); }
        
        //input.CallRunEvent(isRun);

        if (IsArrived())
        {
            SetSearchState();
        }

        Invoke(nameof(SetWanderState), searchTime);
    }

    /// <summary>
    /// 도망 상태. 방황 상태에서 체력이 낮을 때 적대 대상을 감지했다면 일정 확률로 도망 상태가 된다. 
    /// 대상으로부터 가장 빨리 멀어지는 방향으로 이동하려 하며, 해당 방향의 경로가 막혀 있다면 가만히 서 있게 된다.
    /// 일정 시간 후에 다시 방황 상태가 된다.
    /// </summary>
    private void RunawayState()
    {
        if (IsArrived())
        {
            SetRunawayState(target);
        }

        Invoke(nameof(SetWanderState), runawayTime);
    }

    /// <summary>
    /// 다음 목적지 좌표를 지정한다.
    /// </summary>
    private void SetNextDestination(Vector3 next)
    {
        curDestination = next;
        agent.destination = curDestination;
    }

    private void SetWaitState()
    {
        target = null;
        agent.speed = moveSpeed;
        aiState = eAIStateType.Wait;
    }

    private void SetWanderState()
    {
        target = null;
        agent.speed = moveSpeed;
        int index = Random.Range(0, WanderDestinations.Count);
        SetNextDestination(WanderDestinations[index]);
        aiState = eAIStateType.Wander;
    }

    /// <summary>
    /// 추적 대상을 설정하고 추적 상태가 되도록 한다.
    /// </summary>
    private void SetChaseState(GameObject enemy)
    {
        target = enemy;
        //agent.speed = moveSpeed * stats.RunMultiplier;
        aiState = eAIStateType.Chase;
        ChangeBGM();
    }

    private void SetAttackState(GameObject enemy)
    {
        target = enemy;
        //agent.speed = moveSpeed * stats.RunMultiplier;
        aiState = eAIStateType.Attack;
    }

    private void SetSearchState()
    {
        if(target == null)
        {

            collider.radius = maxDetectRadius;
            //agent.speed = moveSpeed * stats.RunMultiplier;
            int index = Random.Range(0, WanderDestinations.Count);
            SetNextDestination(WanderDestinations[index]);
            aiState = eAIStateType.Search;
        }
    }

    /// <summary>
    /// 타겟으로부터 멀어지는 방향으로 움직이게 만들고 도망 상태가 되도록 한다. 해당 방향으로 진행할 수 없다면 가만히 서 있게 한다.
    /// </summary>
    private void SetRunawayState(GameObject enemy)
    {
        target = enemy;
        //agent.speed = moveSpeed * stats.RunMultiplier;
        Vector3 enemyPosition = enemy.transform.position;
        Vector3 aiPosition = transform.position;

        Vector3 runDestination = new Vector3(aiPosition.x - enemyPosition.x, 0f, aiPosition.z - enemyPosition.z).normalized * 3f;

        NavMeshPath nmp = new NavMeshPath();

        //갈 수 없는 곳이라면 가만히 서 있는다.
        if (!agent.CalculatePath(runDestination, nmp))
        {
            runDestination = transform.position;
        }

        SetNextDestination(runDestination);
        //aiState = eAIStateType.Runaway;
    }

    /// <summary>
    /// Collider 를 추적 범위로 이용한다. 범위 내에 들어온 대상이 플레이어라면 추적 상태로 전환한다.
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        //추적 대상이 없을 때 대상이 플레이어라면 
        if (target == null && other.gameObject.CompareTag("Pilot"))
        {
            ////체력이 일정 비율 이하일 때 + 일정 확률로 도망 상태가 된다
            //if (stats.Hp < (stats.MaxHp * 0.2f))
            //{
            //    if(Random.Range(0,1) < 0.3f)
            //    {
            //        SetRunawayState(other.gameObject);
            //        return;
            //    }
            //}
            //SetChaseState(other.gameObject);
        }
    }


    private void DoAttack()
    {
        if(aiState == eAIStateType.Attack)
        {
            aimDirection = new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.z - this.transform.position.z);
            //input.CallAttackEvent(aimDirection);
        }
    } 

    public void OnDead()
    {
        if (aiState == eAIStateType.Chase || aiState == eAIStateType.Attack)
        {
            //SoundManager.Instance.Play("Effect/Minifantasy_Forgotten_Plains_SFX/14_Wind_loop", eSoundType.Ambient);
        }
    }

    /// <summary>
    /// 현재 목적지에 매우 근접했다면 true, 아니면 false
    /// </summary>
    private bool IsArrived()
    {
        if (Vector3.Distance(transform.position, curDestination) < characterRadius)
        {
            return true;
        }

        return false;
    }

    private void ChangeBGM()
    {
        if(aiState == eAIStateType.Chase)
        {
            //SoundManager.Instance.Play("Effect/Shapeforms Audio Free Sound Effects/Dystopia – Ambience and Drone Preview/AUDIO/AMBIENCE_HEARTBEAT_LOOP", eSoundType.Ambient);
        }
        else
        {
            //SoundManager.Instance.Play("Effect/Minifantasy_Forgotten_Plains_SFX/14_Wind_loop", eSoundType.Ambient);
        }
    }
}
