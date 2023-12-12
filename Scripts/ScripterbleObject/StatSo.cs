using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Stat")]
public class StatSo : ScriptableObject
{
     public int defense;
     public int health;
     public int attackPower;


    
    // 방어력에 대한 속성
    public int Defense
    {
       get { return defense; } set{} 
    }

    // 체력에 대한 속성
    public int Health
    {
       get { return health; }
        set { health = Mathf.Max(health ,value); }
    }

    // 공격력에 대한 속성
    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = Mathf.Max(0, value); } // 음수 방지
    }
}
