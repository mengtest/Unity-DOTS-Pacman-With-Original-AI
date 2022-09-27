using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyType
{
    inky,
    pinky,
    blinky,
    clyde
}

[GenerateAuthoringComponent]
public struct TargetData : IComponentData
{
    public float3 target;
    public EnemyType enemyType;
}
