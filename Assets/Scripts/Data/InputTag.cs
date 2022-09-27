using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct InputTag : IComponentData
{
    public Dir mainDir;
    public Dir predDir;
}
