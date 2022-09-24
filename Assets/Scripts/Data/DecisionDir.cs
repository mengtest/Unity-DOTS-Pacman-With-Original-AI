using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Flags]
public enum Dir
{
    up = 1,
    left = 2,
    down = 4,
    right = 8
}

[GenerateAuthoringComponent]
public struct DecisionDir : IBufferElementData
{
    public Dir dir;
}
