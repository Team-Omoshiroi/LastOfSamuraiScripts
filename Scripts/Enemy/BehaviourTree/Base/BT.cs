using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using Characters.Player;
using UnityEngine;
using UnityEngine.AI;

public abstract class BT : MonoBehaviour
{
    private BTNode root;
    private EnemyStatusController enemyStats;
    private PlayerStatusController playerStats;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Animator anime;
    [SerializeField] private NavMeshAgent agent;

    [Header("이동/플레이어 감지/순찰 관련")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float fovRange = 6f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float recognitionTime = 0.5f;

    [Header("일반 공격/특수 공격 관련")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackPerSecond = 1f;
    [SerializeField] private float coolDownTime_SA = 0.3f;
    [SerializeField] private float frequency_SA = 0.3f;
    [SerializeField] private float unstoppableTime = 0.4f;
    [SerializeField] protected bool isUnstoppable = false;
    [SerializeField] private EnemyWeapon[] weapons;

    [Header("사망/피격 관련")]
    [SerializeField] private float groggyTime = 1f;
    [SerializeField] private float hitDelayTime = 0.2f;
    [SerializeField] private float invincibleTime = 0.2f;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isInvincible = false;

    [Header("패링 관련")]
    [SerializeField] private float frequency_parrying = 0.8f;
    [SerializeField] private float parryingTime = 0.2f;
    [SerializeField] protected bool isParrying = false;

    [Header("패링당함 등 무력화 관련")]
    [SerializeField] private float defensibleTime = 0.2f;
    [SerializeField] protected bool isDefensible = false;


    private AnimatorControllerParameter[] animeParams;

    protected float groggyCounter = 0f;
    protected bool isGroggy = false;
    protected float hitDelayCounter = 0f;
    protected bool isHitDelay = false;
    protected float defensibleCounter = 0f;
    protected float unstoppableCounter = 0f;
    protected float invinvibleCounter = 0f;
    protected float parryingCounter = 0f;

    public Transform[] Waypoints { get { return waypoints; } protected set { waypoints = value; } }
    public Animator Anime { get { return anime; } protected set { anime = value; } }
    public NavMeshAgent Agent { get { return agent; } protected set { agent = value; } }
    public float MoveSpeed { get { return moveSpeed; } protected set { moveSpeed = value; } }
    public float FovRange { get { return fovRange; } protected set { fovRange = value; } }
    public float AttackRange { get { return attackRange; } protected set { attackRange = value; } }
    public float AttackPerSecond { get { return attackPerSecond; } protected set { attackPerSecond = value; } }
    public float PatrolWaitTime { get { return patrolWaitTime; } protected set { patrolWaitTime = value; } }
    public float RecognitionTime { get { return recognitionTime; } protected set { recognitionTime = value; } }
    public float CoolDownTime_SA { get { return coolDownTime_SA; } protected set { coolDownTime_SA = value; } }
    public float Frequency_SA { get { return frequency_SA; } protected set { frequency_SA = value; } }
    public float Frequency_parrying { get { return frequency_parrying; } protected set { frequency_parrying = value; } }
    public float ParryingTime { get { return parryingTime; } protected set { parryingTime = value; } }
    public float GroggyTime { get { return groggyTime; } protected set { groggyTime = value; } }
    public float HitDelayTime { get { return hitDelayTime; } protected set { hitDelayTime = value; } }
    public float DefensibleTime { get { return defensibleTime; } protected set { defensibleTime = value; } }
    public float UnstoppableTime { get { return unstoppableTime; } protected set { unstoppableTime = value; } }
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public bool IsDefensible { get { return isDefensible; } set { isDefensible = value; } }
    public bool IsUnstoppable { get { return isUnstoppable; } set { isUnstoppable = value; } }
    public bool IsParrying { get { return isParrying; } set { isParrying = value; } }
    public bool IsArmed { get; set; }

    public BTNode Root { get { return root; } protected set { root = value; } }
    public EnemyStatusController EnemyStats { get { return enemyStats; } protected set { enemyStats = value; } }
    public PlayerStatusController PlayerStats { get { return playerStats; } protected set { playerStats = value; } }

    // Start is called before the first frame update
    protected void Start()
    {
        enemyStats = GetComponent<EnemyStatusController>();
        playerStats = PlayerStatusController.Instance;
        animeParams = anime.parameters;
        SetWeaponDamage();
        SetupTree();
    }

    // Update is called once per frame
    protected void Update()
    {
        PrepareWeapon();

        if (root != null)
        {
            root.Evaluate();
        }
    }

    protected abstract void SetupTree();

    /// <summary>
    /// 모든 애니메이션 파라미터를 초기화한다.
    /// </summary>
    public void ResetAnimations()
    {
        AnimatorControllerParameterType paramtype;

        for(int i = 0; i< animeParams.Length; i++)
        {
            paramtype = animeParams[i].type;
            switch (paramtype)
            {
                case AnimatorControllerParameterType.Bool:
                    {
                        anime.SetBool(animeParams[i].name, false);
                        break;
                    }
                case AnimatorControllerParameterType.Int:
                    {
                        anime.SetInteger(animeParams[i].name, 0);
                        break;
                    }
                case AnimatorControllerParameterType.Float:
                    {
                        anime.SetFloat(animeParams[i].name, 0f);
                        break;
                    }
                case AnimatorControllerParameterType.Trigger:
                    {
                        anime.ResetTrigger(animeParams[i].name);
                        break;
                    }
            }
        }

        //anime.Rebind();
    }

    //즉시 해당 애니메이션 상태로 전환한다.
    public void SwitchAnimation(string AnimationStateName)
    {
        anime.CrossFade(AnimationStateName, 0.0f);
    }

    /// <summary>
    /// float 파라미터는 음수 입력 시 기존 값으로 유지됨.
    /// </summary>
    public void Initialize(Transform[] waypoints, float moveSpeed, float fovRange, float attackRange, float attackPerSecond, float recognitionTime, float coolDownTime_SA, float frequency_SA)
    {
        this.waypoints = waypoints;

        if(moveSpeed >= 0)
            this.moveSpeed = moveSpeed;
        if(fovRange >= 0)
            this.fovRange = fovRange;
        if(attackRange >= 0)
            this.attackRange = attackRange;
        if(attackPerSecond >= 0)
            this.attackPerSecond = attackPerSecond;
        if(recognitionTime >= 0)
            this.recognitionTime = recognitionTime;
        if (coolDownTime_SA >= 0)
            this.coolDownTime_SA = coolDownTime_SA;
        if(frequency_SA >= 0)
            this.frequency_SA = frequency_SA;
    }

    public void PrepareWeapon()
    {
        if (!IsArmed)
        {
            ResetAnimations();
            anime.Rebind();
            anime.SetBool("PrepareWeapon", true);
            IsArmed = true;
        }
    }

    public void Dead()
    {
        isDead = true;
    }

    private void SetWeaponDamage()
    {
        foreach (EnemyWeapon weapon in weapons)
        {
            weapon.Damage = enemyStats.AP;
        }
    }

    public void ActivateWeapon()
    {
        foreach (EnemyWeapon weapon in weapons)
        {
            weapon.isAttacking = true;
        }
    }
}
