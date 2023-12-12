using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [Serializable]
    public class AttackInfoData
    {
        [field: SerializeField] public string AttackName { get; private set; }
        [field: SerializeField] public int ComboStateIndex { get; private set; }
        [field: SerializeField][field: Range(0f, 1f)] public float MinComboTransitionTime { get; private set; }
        [field: SerializeField][field: Range(0f, 1f)] public float MaxComboTransitionTime { get; private set; }

        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public GameObject Effect { get; private set; }
    }


    [Serializable]
    public class PlayerAttackData 
    {
        [field: SerializeField] public List<AttackInfoData> AttackInfoDatas { get; private set; }
        public int GetAttackInfoCount() { return AttackInfoDatas.Count; }
        public AttackInfoData GetAttackInfo(int index) { return AttackInfoDatas[index]; }
    }
}