using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PacdotSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        EntityCommandBuffer.ParallelWriter ecbParallel = ecb.AsParallelWriter();
        Translation translation = new Translation { Value = float3.zero };
        Entities.WithAll<InputTag>().ForEach((ref Translation translation1) => { translation = translation1; }).WithBurst().Run();
        Entities
            .WithAll<PacdotTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation1) => {
                if (math.distance(translation.Value,translation1.Value) <= 0.5f)
                {
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }
            }).WithBurst().ScheduleParallel();

        this.Dependency.Complete();

        ecb.Playback(this.EntityManager);

        ecb.Dispose();
    }
}
