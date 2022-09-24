using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct Calculate : IJobEntity
{
    public NativeArray<DecisionLocData> decisionLocData;
    public NativeArray<float> OutData;
    void Execute([EntityInQueryIndex] int entityInQueryIndex, ref Translation translation)
    {
        OutData[entityInQueryIndex] = math.distance(decisionLocData[entityInQueryIndex].Loc, translation.Value);
    }
}

public partial class AISystem : SystemBase
{
    protected override void OnUpdate()
    {
        
        float elapsedTime = (float)Time.ElapsedTime;
        Entities.WithAll<AIData>()
            .ForEach((ref AIData aiData, ref PhysicsVelocity physicsVelocity , ref Translation translation, in DynamicBuffer<DecisionLocData> DecisionLoc, in DynamicBuffer<DecisionDir> DecisionDir) =>
            {
                
                DecisionPoint(ref physicsVelocity, ref translation, in DecisionLoc, ref aiData, DecisionDir);

               
                /*switch (aiData.state)
                {
                    case State.Wait:
                        Wait(ref aiData, ref physicsVelocity,in translation, in elapsedTime);
                        break;
                    case State.Init:
                        
                        break;
                    case State.Scatter:
                        break;
                    case State.Chase:
                        break;
                    case State.Run:
                        break;
                    default:
                        break;
                }*/
            }).WithoutBurst().Run();
    }

    static void Wait(ref AIData aIData,  ref PhysicsVelocity physicsVelocity, in Translation translation, in float elapsedTime)
    {
        if (elapsedTime >= aIData.time)
        {
            
        }
    }
    static void DecisionPoint(ref PhysicsVelocity physicsVelocity, ref Translation translation, in DynamicBuffer<DecisionLocData> DecisionLoc, ref AIData aIData , in DynamicBuffer<DecisionDir> decisionDirData)
    {
        float UpDis = 0;
        float DownDis = 0;
        float LeftDis = 0;
        float RightDis = 0;
        float max = float.MaxValue;
        for (int i = 0; i < DecisionLoc.Capacity; i++)
        {
            if (math.distance(DecisionLoc[i].Loc,translation.Value) <= 0.0001f)
            {
                if (decisionDirData[i].dir.HasFlag(Dir.up))
                {
                    
                    if (aIData.MainDir != Dir.down)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.y += 0.5f;
                        UpDis = math.distance(DirLoc, aIData.target);
                        if (UpDis < max)
                        {
                            max = UpDis;
                        }
                    }
                }
                if (decisionDirData[i].dir.HasFlag(Dir.down))
                {
                    if (aIData.MainDir != Dir.up)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.y += -0.5f;
                        DownDis = math.distance(DirLoc, aIData.target);
                        if (DownDis < max)
                        {
                            max = DownDis;
                        }
                    }
                }
                if (decisionDirData[i].dir.HasFlag(Dir.left))
                {
                    if (aIData.MainDir != Dir.right)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.x += -0.5f;
                        LeftDis = math.distance(DirLoc, aIData.target);
                        if (LeftDis < max)
                        {
                            max = LeftDis;
                        }
                    }
                }
                if (decisionDirData[i].dir.HasFlag(Dir.right))
                {
                    if (aIData.MainDir != Dir.left)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.x += 0.5f;
                        RightDis = math.distance(DirLoc, aIData.target);
                        if (RightDis < max)
                        {
                            max = RightDis;
                        }
                    }
                }
                break;
            }
        }

        if (max == UpDis)
        {
            aIData.MainDir = Dir.up;
        }
        else if (max == DownDis)
        {
            aIData.MainDir = Dir.down;
        }
        else if (max == LeftDis)
        {
            aIData.MainDir = Dir.left;
        }
        else if (max == RightDis)
        {
            aIData.MainDir = Dir.right;
        }

        switch (aIData.MainDir)
        {
            case Dir.up:
                physicsVelocity.Linear.y = 1f;
                physicsVelocity.Linear.x = 0f;
                break;
            case Dir.left:
                physicsVelocity.Linear.x = -1f;
                physicsVelocity.Linear.y = 0f;
                break;
            case Dir.down:
                physicsVelocity.Linear.y = -1f;
                physicsVelocity.Linear.x = 0f;
                break;
            case Dir.right:
                physicsVelocity.Linear.x = 1f;
                physicsVelocity.Linear.y = 0f;
                break;
            default:
                break;
        }
    }
}
