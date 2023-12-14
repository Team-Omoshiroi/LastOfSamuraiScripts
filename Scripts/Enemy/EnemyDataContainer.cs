using Characters.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataContainer : MonoBehaviour
{
    [SerializeField] private BaseAI ai;
    [SerializeField] private EnemyStatusController statsController;

    public BaseAI AI { get { return ai; } protected set { ai = value; } }
    public EnemyStatusController StatsController { get { return statsController; } protected set { statsController = value; } }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Initialize()
    {
        statsController.OnDead += () => Dead();
    }

    private void Dead()
    {
        AI.IsDead = true;
        DropItem();
    }

    private void DropItem()
    {

    }
}
