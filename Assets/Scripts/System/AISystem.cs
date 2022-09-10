using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial class AISystem : SystemBase
{
    const int zero = 0;
    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;
        Entities.WithAll<AIData>()
            .ForEach((ref AIData aiData, ref DynamicBuffer<WaypointBuffer> waypoints, ref PhysicsVelocity physicsVelocity , in Translation translation) =>
            {
                switch (aiData.state)
                {
                    case State.Wait:
                        Wait(ref aiData, ref waypoints,ref physicsVelocity,in translation, in elapsedTime);
                        break;
                    case State.Init:
                        MoveToWaypoint(ref waypoints, ref physicsVelocity, in translation);
                        break;
                    case State.Scatter:
                        break;
                    case State.Chase:
                        break;
                    case State.Run:
                        break;
                    default:
                        break;
                }
            }).WithBurst().ScheduleParallel();
    }

    static void Wait(ref AIData aIData, ref DynamicBuffer<WaypointBuffer> waypoints, ref PhysicsVelocity physicsVelocity, in Translation translation, in float elapsedTime)
    {
        if (elapsedTime >= aIData.time)
        {
            aIData.state = State.Init;
            if (aIData.time > zero)
            {
                waypoints.RemoveRange(zero, waypoints.Length);
            }
            InitializeWaypoints(in aIData, ref waypoints);
        }
    }

    static void InitializeWaypoints(in AIData aIData, ref DynamicBuffer<WaypointBuffer> waypoints)
    {
        if (aIData.state == State.Init || aIData.state == State.Scatter)
        {
            var buffer = new WaypointBuffer();
            buffer.waypoint = aIData.dfWaypoint1;   
            waypoints.Add(buffer);
            var buffer2 = new WaypointBuffer();
            buffer2.waypoint = aIData.dfWaypoint2;
            waypoints.Add(buffer2);
            var buffer3 = new WaypointBuffer();
            buffer3.waypoint = aIData.dfWaypoint3;
            waypoints.Add(buffer3);
            var buffer4 = new WaypointBuffer();
            buffer4.waypoint = aIData.dfWaypoint4;
            waypoints.Add(buffer4);
            var buffer5 = new WaypointBuffer();
            buffer5.waypoint = aIData.dfWaypoint5;
            waypoints.Add(buffer5);
            var buffer6 = new WaypointBuffer();
            buffer6.waypoint = aIData.dfWaypoint6;
            waypoints.Add(buffer6);
        }
    }
    static void MoveToWaypoint(ref DynamicBuffer<WaypointBuffer> waypoints,ref PhysicsVelocity physicsVelocity, in Translation translation)
    {
        if (waypoints.Length == 0) {
            physicsVelocity.Linear.x =  0;
            physicsVelocity.Linear.y =  0;
            
            return;
        }
        var waypoint = waypoints[0].waypoint;
        float2 translation2;
        translation2.x = translation.Value.x;
        translation2.y = translation.Value.y;
        if (math.distance(waypoint, translation2) > 0.1f)
        {                                                           
            float2 _direction;
            _direction.x = waypoint.x - translation.Value.x; 
            _direction.y = waypoint.y - translation.Value.y; 
            _direction = math.normalize(_direction);
            physicsVelocity.Linear.x = _direction.x > 0.5f || _direction.x < -0.5f ? _direction.x : 0;
            physicsVelocity.Linear.y = _direction.y > 0.5f || _direction.y < -0.5f ? _direction.y : 0;

        }
        else
        {
            waypoints.RemoveAt(0);
        }
    }
}
