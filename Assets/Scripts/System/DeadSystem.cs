using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
[UpdateAfter(typeof(InputSystem))]
public partial class DeadSystem : SystemBase
{

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        EntityCommandBuffer.ParallelWriter ecbParallel = ecb.AsParallelWriter();
        Translation? translation = null;
        Entity? entity = null;
        Entities.WithAll<InputTag>().ForEach((Entity entity1, ref Translation translation1) => { translation = translation1; entity = entity1; }).WithBurst().Run();
        Entities
            .WithAll<AIData>()
            .ForEach((int entityInQueryIndex, in Translation translation1) => {
                if (math.distance(translation.Value.Value, translation1.Value) <= 1f)
                {
                    ecbParallel.AddComponent<DeadTag>(entityInQueryIndex, entity.Value);
                }
            }).WithBurst().ScheduleParallel();

        Entities
            .WithAll<DeadTag>()
            .ForEach((ref MoveData moveData,in AnimatorData animatorData) => {
                animatorData.animator.SetBool("Die", true);
                moveData.moveDir.x = 0f;
                moveData.moveDir.y = 0f;
            }).WithoutBurst().Run();

        this.Dependency.Complete();

        ecb.Playback(this.EntityManager);

        ecb.Dispose();
        
    }
}
