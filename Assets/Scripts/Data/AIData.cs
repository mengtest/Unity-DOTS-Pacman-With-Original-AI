using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[SerializeField]
public enum State { Wait, Init, Scatter, Chase, Run };
[GenerateAuthoringComponent]
public struct AIData : IComponentData
{
    public State state;
    public float2 dfWaypoint1;
    public float2 dfWaypoint2;
    public float2 dfWaypoint3;
    public float2 dfWaypoint4;
    public float2 dfWaypoint5;
    public float2 dfWaypoint6;
    public float time;
}

