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
    public float time;
    public float3 target;
    public Dir MainDir;
    public Dir PrevDir;
    public float Speed;
    public int limitWaypoint;
}

