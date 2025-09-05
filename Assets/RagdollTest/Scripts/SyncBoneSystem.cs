 
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
partial struct SyncBoneSystem : ISystem
{
    [BurstCompile]
    partial struct SyncBoneJob : IJobEntity
    { 
        void Execute( ref RagdollBoneData             boneData,
                      EnabledRefRW<RagdollBoneData>   boneEnableState, 
                      LocalTransform                  transform )
        {
            if(boneData.m_BlendFactor<=0.0f)
            {
               boneEnableState.ValueRW = false;
               return;            
            }
            boneData.m_BlendFactor -= 0.1f;
            boneData.m_Position = transform.Position;
            boneData.m_Rotation = transform.Rotation;
        }
    }


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
      
    }

    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new SyncBoneJob().ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
