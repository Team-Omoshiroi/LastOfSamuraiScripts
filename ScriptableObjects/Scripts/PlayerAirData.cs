using System;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [Serializable]
    public class PlayerAirData
    {
        [field: Header("JumpData")]
        [field: SerializeField][field: Range(0f, 25f)] public float JumpForce { get; private set; } = 4f;

    }
}