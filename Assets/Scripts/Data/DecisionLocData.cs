using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[GenerateAuthoringComponent]
public struct DecisionLocData : IBufferElementData
{
    public float3 Loc;
}
