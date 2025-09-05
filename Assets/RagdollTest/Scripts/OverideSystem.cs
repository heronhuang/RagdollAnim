using Rukhanka;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(RukhankaAnimationInjectionSystemGroup))]
[RequireMatchingQueriesForUpdate]
partial struct OverideSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }
    [BurstCompile]
    partial struct BonePositionOverrideJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction]
        public RuntimeAnimationData runtimeData;
        [ReadOnly]
        public ComponentLookup<RigDefinitionComponent> rigDefLookup;
        void Execute(   RagdollBoneData            bonePosOverride, 
                        AnimatorEntityRefComponent boneRef,
                        RagdollMasterData         masterData,
                        ref LocalTransform         transform,                     
                        ref PhysicsVelocity        vec )
        {

            RigDefinitionComponent rigDef = rigDefLookup[masterData.MasterEntity];
          
            using var animStream = AnimationStream.Create(runtimeData, masterData.MasterEntity, rigDef);


            var pose = animStream.GetWorldPose(boneRef.boneIndexInAnimationRig);
            
            //Factor 1.0 to be Ragdoll's position ,0.0f to skeleton's position
            var position = math.lerp  ( pose.pos, bonePosOverride.m_Position,   bonePosOverride.m_BlendFactor);
            var rotation = math.slerp ( pose.rot, bonePosOverride.m_Rotation,   bonePosOverride.m_BlendFactor);

            vec.Linear   = math.lerp(vec.Linear, 0, bonePosOverride.m_BlendFactor);
            vec.Angular  = math.lerp(vec.Angular, 0, bonePosOverride.m_BlendFactor);
            
            transform.Position = position;
            transform.Rotation = rotation;
            animStream.SetWorldPosition ( boneRef.boneIndexInAnimationRig, position );
            animStream.SetWorldRotation ( boneRef.boneIndexInAnimationRig, rotation );
            
            // Implicit animationStream.Dispose() call is here
        }
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ref var runtimeData = ref SystemAPI.GetSingletonRW<RuntimeAnimationData>().ValueRW;
        var job = new BonePositionOverrideJob()
        {
            runtimeData = runtimeData,
            rigDefLookup = SystemAPI.GetComponentLookup<RigDefinitionComponent>(true)
        };

        job.ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
