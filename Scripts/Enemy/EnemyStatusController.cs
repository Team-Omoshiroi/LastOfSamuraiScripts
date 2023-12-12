using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusController : BaseStatusController
{
    StatSo Enumystat;
    public event Action OnDead;


    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        AP = Enumystat.AttackPower;
        MaxHp = Enumystat.Health;
        CurHp = Enumystat.Health;
        Dp = Enumystat.Defense;
    }
    public void TakeDamage(int damageAmount)
    {       
        float defenseEffect = Mathf.Min(Dp / 100f, maxDefenseEffect); 

        // 방어력을 백분율로 계산한 실제 피해량
        int actualDamage = damageAmount - (int)(damageAmount * defenseEffect);

        CurHp -= actualDamage;

        if (CurHp <= 0)
        {
            Die(); 
        }
      
    }
    private void Die()
    {
        
        Destroy(this.gameObject);
    }
}
