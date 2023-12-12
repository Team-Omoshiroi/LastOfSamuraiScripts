using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "EnemyStat", menuName = "Characters/EnemyStat")]
    public class EnemySO : ScriptableObject
    {
        public string Name { get; private set; }
        public float MaxHp { get; private set; }
        public float MoveSpeed { get; private set; }
        public float FovRange { get; private set; }
        public float AttackRange { get; private set; }
        public float AttackPerSecond { get; private set; }
        public float PatrolWaitTime { get; private set; }
        public float RecognitionTime { get; private set; }
        public float GroggyTime { get; private set; }
        public float HitDelayTime { get; private set; }
    }
}