using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct FireSystem : ISystem 
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<FireData>();
    }
    public void OnUpdate(ref SystemState state)
    {

        if (Input.GetMouseButtonDown(0))
        {
            var fireData = SystemAPI.GetSingleton<FireData>();

            SystemAPI.TryGetSingletonEntity<FireData>(out Entity entity);
            LocalTransform firelocalTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);
            LocalToWorld fireLocalToWorld = state.EntityManager.GetComponentData<LocalToWorld>(entity);

            float3 position = fireLocalToWorld.Position + fireLocalToWorld.Forward * fireData.BULLET_FORWARD_OFFSET + fireLocalToWorld.Up * fireData.BULLET_UP_OFFSET;
            quaternion rotate = fireLocalToWorld.Rotation;

            Entity bulletEntity = state.EntityManager.Instantiate(fireData.mBulletTemplate);
            LocalTransform bulletLocalTransform =  state.EntityManager.GetComponentData<LocalTransform>(bulletEntity);
            bulletLocalTransform.Position = position;
            bulletLocalTransform.Rotation = rotate;
            SystemAPI.SetComponent(bulletEntity, bulletLocalTransform);

            //物理
            float3 force = math.normalize(fireLocalToWorld.Forward + fireLocalToWorld.Up * 0.5f) * fireData.BULLET_SPEED;
            PhysicsVelocity physicsVelocity = state.EntityManager.GetComponentData<PhysicsVelocity>(bulletEntity);
            physicsVelocity.Linear += force;
            SystemAPI.SetComponent(bulletEntity, physicsVelocity);
        }
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {


    }
}
