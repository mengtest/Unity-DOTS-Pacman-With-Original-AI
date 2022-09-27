using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
[UpdateBefore(typeof(AISystem))]
public partial class TargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Translation translationPacman = new Translation();
        InputTag inputTag = new InputTag();
        Entities.WithAll<InputTag>().ForEach((ref Translation translation1, ref InputTag inputTag1) => { translationPacman = translation1; inputTag = inputTag1; }).WithBurst().Run();
        
        float3 targetPinky = float3.zero;
        float3 blinkyLoc = float3.zero;
        Entities.WithAll<AIData>()
            .ForEach((ref TargetData targetData,in Translation translation1, in AIData aIData ) =>
            {
                switch (targetData.enemyType)
                {
                    case EnemyType.inky:

                        break;
                    case EnemyType.pinky:
                        switch (inputTag.mainDir)
                        {
                            case Dir.up:
                                var temp = translationPacman.Value;
                                temp.x += -3f;
                                temp.y += 4f;
                                targetData.target = temp;
                                targetPinky = temp;
                                break;
                            case Dir.left:
                                var temp2 = translationPacman.Value;
                                temp2.x += -4f;
                                targetData.target = temp2;
                                targetPinky = temp2;
                                break;
                            case Dir.down:
                                var temp3 = translationPacman.Value;
                                temp3.y += -4f;
                                targetData.target = temp3;
                                targetPinky = temp3;
                                break;
                            case Dir.right:
                                var temp4 = translationPacman.Value;
                                temp4.x += 4f;
                                targetData.target = temp4;
                                targetPinky = temp4;
                                break;
                            default:
                                break;
                        }
                        break;
                    case EnemyType.blinky:
                        targetData.target = translationPacman.Value;
                        blinkyLoc = translation1.Value;
                        break;
                    case EnemyType.clyde:
                        if (math.distance(translationPacman.Value, translation1.Value) <= 4f)
                        {
                            targetData.target = aIData.target;
                        }
                        else
                        {
                            targetData.target = translationPacman.Value;
                        }
                        break;
                    default:
                        break;
                }
            }).WithBurst().Run();

        

        
        Entities.WithAll<AIData>()
            .ForEach((ref TargetData targetData, in Translation translation1, in AIData aIData) =>
            {
                switch (targetData.enemyType)
                {
                    case EnemyType.inky:
                        targetData.target = Rotate(in blinkyLoc, in targetPinky);
                        break;
                    default:
                        break;
                }
            }).WithBurst().Run();
    }

    static float3 Rotate(in float3 blinkyLoc, in float3 targetPinky)
    {
        float3 vector = blinkyLoc - targetPinky;
        vector.x = vector.x * math.cos(math.radians(180f)) - vector.y * math.sin(math.radians(180f));
        vector.y = vector.x * math.sin(math.radians(180f)) + vector.y * math.cos(math.radians(180f));
        return vector;
    }
}
