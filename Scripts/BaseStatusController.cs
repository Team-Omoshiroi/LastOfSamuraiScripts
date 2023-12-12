using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatusController : MonoBehaviour
{
    public int AP;
    public int Dp;
    public int MaxHp;
    public int CurHp;
    public float maxDefenseEffect = 0.75f;  // 방어력의 최대 효과를 75%로 제한
}
