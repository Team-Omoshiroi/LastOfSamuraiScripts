using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "Player", menuName = "Characters/Player")]
    public class PlayerSo : ScriptableObject
    {
        [field:SerializeField] public PlayerGroundData GroundData { get; private set; }
        [field:SerializeField] public PlayerAirData AirData { get; private set; }
        [field:SerializeField] public PlayerAttackData AttackData { get; private set; }
    }
}