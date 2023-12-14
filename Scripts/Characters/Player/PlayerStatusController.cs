using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PlayerStatusController : BaseStatusController
{
    public event Action OnDead;

    public Slider HpBar;
    public GameObject dropRoot;
    public StatSo stat;
    public static PlayerStatusController Instance { get; set; }




  
 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스가 있으면 제거
        }
        else
        {
           Instance = this;
         
        }
        
        Init();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Init()
    {
        AP = stat.AttackPower;
        MaxHp = stat.Health;
        CurHp = stat.Health;
        Dp = stat.Defense;
    }
    private void UpdateUI()
    {
       HandlerHp(MaxHp, CurHp);
    }
    public void IncreaseHealth(int value)
    {
        CurHp = Mathf.Max(0, CurHp + value);
        UpdateUI();
    }

    public void HandlerHp(float MaxHp, float CurHp)
    {
        if (MaxHp > 0)
        {
            HpBar.value = CurHp / MaxHp;
        }
        else
        {
           
            HpBar.value = 0; 
        }

    }
    public void TakeDamage(int damageAmount)
    {
     
       
        float defenseEffect = Mathf.Min(Dp / 100f, maxDefenseEffect); 

        // 방어력을 백분율로 계산한 실제 피해량
        int actualDamage =  damageAmount - (int)(damageAmount * defenseEffect);

        CurHp -= actualDamage;
        HandlerHp(MaxHp, CurHp);
        if (CurHp <= 0)
        {
            Die(); 
        }
        
    }

    private void Die()
    {
        //todo  먼가 .. 부족
        Destroy(this.gameObject);
    }
}
