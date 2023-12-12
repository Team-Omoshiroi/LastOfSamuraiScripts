using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMapGraphicSetting : MonoBehaviour
{
    [field: SerializeField][field: Range(10, 200)] public int TargetFrameRate { get; private set; } = 60;
    private void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }
}
