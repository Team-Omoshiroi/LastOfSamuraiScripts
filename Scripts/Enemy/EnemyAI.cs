using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject targetObject;

    private BTNode root;

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
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 viewDirection = new Vector2();
    private Vector2 aimDirection = new Vector2();
    private Vector3 curDestination;
    private GameObject target;

    private float characterRadius;

    public List<Vector3> WanderDestinations { get { return wanderDestinations; } private set { wanderDestinations = value; } }
    public NavMeshAgent Agent { get { return agent; } private set { agent = value; } }
    public float NormDetectRadius { get { return normDetectRadius; } private set { normDetectRadius = value; } }
    public float MaxDetectRadius { get { return MaxDetectRadius; } private set { MaxDetectRadius = value; } }
    public float ChaseDistance { get { return chaseDistance; } private set { chaseDistance = value; } }
    public float AttackDistance { get { return attackDistance; } private set { attackDistance = value; } }
    public float AttackRange { get { return attackRange; } private set { attackRange = value; } }
    public float MoveSpeed { get { return moveSpeed; } private set { moveSpeed = value; } }

    public Vector2 ViewDirection { get { return viewDirection; } private set { viewDirection = value; } }
    public Vector2 AimDirection { get { return aimDirection; } private set { aimDirection = value; } }
    public Vector3 CurDestination { get { return curDestination; } private set { curDestination = value; } }

    // Start is called before the first frame update
    void Start()
    {
        root = new BTSelector();
        BTSequence deadSequence = new BTSequence();
        BTSelector aliveSelector = new BTSelector();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
