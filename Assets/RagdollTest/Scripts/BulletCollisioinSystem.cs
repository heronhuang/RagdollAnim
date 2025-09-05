
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
  
 

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))] 
partial struct BulletCollisioinSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public partial struct ProjectileJob : IJobEntity
    {

        [ReadOnly] public PhysicsWorld PhysicsWorld;
        public      float deltaTime;
        //[NativeDisableParallelForRestriction]
        [NativeDisableParallelForRestriction]
        public ComponentLookup<RagdollBoneData> RagdollPartDatas;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;


        public void Execute( Entity entity,
                              in LocalTransform transform,
                              in PhysicsMass mass,
                              in BulletData bullet)
        {

            PhysicsVelocity velocity = PhysicsVelocityData[entity];
            float speed = math.length(velocity.Linear);
            if (speed > 0)
            {
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(10, Allocator.Temp);
                if (PhysicsWorld.SphereCastAll( transform.Position,
                                                 0.15f,
                                                 math.normalize(velocity.Linear),
                                                 speed * deltaTime,
                                                 ref hits,
                                                 CollisionFilter.Default,
                                                 QueryInteraction.IgnoreTriggers))
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hit = hits[i];
                        //float3 pos = hit.Position;
                        //float3 normal = hit.SurfaceNormal;
                        if (hits[i].Entity == entity)//过滤自己
                        {
                            continue;
                        }
                        if (RagdollPartDatas.HasComponent(hits[i].Entity))
                        {
                            RagdollPartDatas[hits[i].Entity] = new RagdollBoneData { m_BlendFactor = 1.0f };

                            RagdollPartDatas.SetComponentEnabled(hits[i].Entity, true);

                            //var vec = PhysicsVelocityData[hits[i].Entity];
                            //vec.ApplyAngularImpulse(mass, math.normalize(velocity.Linear)*0.001f);
                            //PhysicsVelocityData[hits[i].Entity] = vec;
                        }
                    }
                }
                hits.Dispose();
            }

        }

    }

    //[BurstCompile]
    //struct CollisionEventImpulseJob : ICollisionEventsJob
    //{
    //    [NativeDisableParallelForRestriction]
    //    public ComponentLookup<RagdollBoneData> RagdollBones;

    //    public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;

    //    public void Execute(CollisionEvent collisionEvent)
    //    {
    //        Entity entityA = collisionEvent.EntityA;
    //        Entity entityB = collisionEvent.EntityB;
    //        bool isBodyADynamic = PhysicsVelocityData.HasComponent(entityA);
    //        bool isBodyBDynamic = PhysicsVelocityData.HasComponent(entityB);  
    //        if (RagdollBones.HasComponent(entityA))
    //        {
    //            RagdollBones[entityA] = new RagdollBoneData { m_BlendFactor = 1.0f };
    //            RagdollBones.SetComponentEnabled(entityA, true);

    //            var vec = PhysicsVelocityData[entityA];

    //            PhysicsVelocityData[entityA] = vec;
    //        }
    //        else if (RagdollBones.HasComponent(entityB))
    //        {
    //            RagdollBones[entityB] = new RagdollBoneData { m_BlendFactor = 1.0f };
    //            RagdollBones.SetComponentEnabled(entityB, true);
    //        }
    //    }
    //}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ProjectileJob()
        {
            PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
            deltaTime = SystemAPI.Time.fixedDeltaTime,
            RagdollPartDatas = state.GetComponentLookup<RagdollBoneData>(),
            PhysicsVelocityData = state.GetComponentLookup<PhysicsVelocity>()

        }.ScheduleParallel();

        //var handle = new CollisionEventImpulseJob()
        //{
        //    RagdollBones = state.GetComponentLookup<RagdollBoneData>(false)

        //}.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        //handle.Complete();

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}



