using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
[AlwaysSynchronizeSystem]
public partial class AISystem : SystemBase
{
    protected override void OnUpdate()
    {
        
        float elapsedTime = (float)Time.ElapsedTime;
        Entities.WithAll<AIData>()
            .ForEach((ref AIData aiData, ref MoveData moveData , ref Translation translation, in DynamicBuffer<DecisionLocData> DecisionLoc, 
            in DynamicBuffer<DecisionDir> DecisionDir, in TargetData targetData) =>
            {
                switch (aiData.state)
                {
                    case State.Wait:
                        Wait(ref aiData, ref moveData, ref translation, in elapsedTime);
                        break;
                    case State.Init:
                        Move(ref moveData, ref translation, in DecisionLoc, ref aiData, in aiData.target, DecisionDir);
                        break;
                    case State.Scatter:
                        break;
                    case State.Chase:
                        Move(ref moveData, ref translation, in DecisionLoc, ref aiData, in targetData.target, DecisionDir);
                        break;
                    case State.Run:
                        break;
                    default:
                        break;
                }
            }).WithBurst().ScheduleParallel();
    }

    static void Wait(ref AIData aIData,  ref MoveData moveData, ref Translation translation, in float elapsedTime)
    {
        if (elapsedTime >= aIData.time)
        {
            translation.Value.y = 1f;
            moveData.moveDir.y = 0f;
            aIData.state = State.Init;
        }
        else
        {
            if (moveData.moveDir.y == 0)
            {
                moveData.moveDir.y = 2f;
            }
            else if ((moveData.moveDir.y > 1f) && (translation.Value.y >= 1.5f))
            {
                moveData.moveDir.y = -2f;
            }
            else if((moveData.moveDir.y < -1f) && (translation.Value.y <= 0.5f))
            {
                moveData.moveDir.y = 2f;
            }
        }
    }
    static void Move(ref MoveData moveData, ref Translation translation, in DynamicBuffer<DecisionLocData> DecisionLoc, ref AIData aIData,
        in float3 target, in DynamicBuffer<DecisionDir> decisionDirData)
    {
        float UpDis = 0;
        float DownDis = 0;
        float LeftDis = 0;
        float RightDis = 0;
        float max = float.MaxValue;
        for (int i = 0; i < DecisionLoc.Capacity; i++)
        {
            if (math.distance(DecisionLoc[i].Loc,translation.Value) <= 0.001f)
            {
                translation.Value = DecisionLoc[i].Loc;
                if ((decisionDirData[i].dir & Dir.up) == Dir.up)
                {
                    
                    if (aIData.MainDir != Dir.down)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.y += 0.5f;
                        UpDis = math.distance(DirLoc, target);
                        if (UpDis < max)
                        {
                            max = UpDis;
                        }
                    }
                }
                if ((decisionDirData[i].dir & Dir.down) == Dir.down)
                {
                    if (aIData.MainDir != Dir.up)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.y += -0.5f;
                        DownDis = math.distance(DirLoc, target);
                        if (DownDis < max)
                        {
                            max = DownDis;
                        }
                    }
                }
                if ((decisionDirData[i].dir & Dir.left) == Dir.left)
                {
                    if (aIData.MainDir != Dir.right)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.x += -0.5f;
                        LeftDis = math.distance(DirLoc, target);
                        if (LeftDis < max)
                        {
                            max = LeftDis;
                        }
                    }
                }
                if ((decisionDirData[i].dir & Dir.right) == Dir.right)
                {
                    if (aIData.MainDir != Dir.left)
                    {
                        float3 DirLoc = translation.Value;
                        DirLoc.x += 0.5f;
                        RightDis = math.distance(DirLoc, target);
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
        if (aIData.PrevDir != aIData.MainDir)
        {
            switch (aIData.MainDir)
            {
                case Dir.up:
                    moveData.moveDir.y = aIData.Speed;
                    translation.Value.y += 0.1f;
                    moveData.moveDir.x = 0f;
                    --aIData.limitWaypoint;
                    break;
                case Dir.left:
                    moveData.moveDir.x = -aIData.Speed;
                    translation.Value.x -= 0.1f;
                    moveData.moveDir.y = 0f;
                    --aIData.limitWaypoint;
                    break;
                case Dir.down:
                    moveData.moveDir.y = -aIData.Speed;
                    translation.Value.y -= 0.1f;
                    moveData.moveDir.x = 0f;
                    --aIData.limitWaypoint;
                    break;
                case Dir.right:
                    moveData.moveDir.x = aIData.Speed;
                    translation.Value.x += 0.1f;
                    moveData.moveDir.y = 0f;
                    --aIData.limitWaypoint;
                    break;
                default:
                    break;
            }
            aIData.PrevDir = aIData.MainDir;
        }

        if (aIData.limitWaypoint <= 0)
        {
            aIData.state = State.Chase;
        }
    }
}
