using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<MoveData>()
            .WithNone<DeadTag>()
            .ForEach((ref Translation translation, in MoveData moveData) => {
                translation.Value += moveData.moveDir * 0.01f;
            }).WithBurst().ScheduleParallel();
    }
}
