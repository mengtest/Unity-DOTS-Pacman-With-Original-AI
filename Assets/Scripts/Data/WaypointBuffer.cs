using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct WaypointBuffer : IBufferElementData
{
    public float2 waypoint;
}
