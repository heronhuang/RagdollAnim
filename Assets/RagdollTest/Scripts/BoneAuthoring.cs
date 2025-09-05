using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RagdollMasterData : IComponentData
{
    public Entity MasterEntity;
}
public struct RagdollBoneData : IComponentData, IEnableableComponent
{
    
    public float3 m_Position;
    public quaternion m_Rotation;
    public float m_BlendFactor;
}


class BoneAuthoring : MonoBehaviour
{
    public GameObject MasterObjet;

    class Baker : Baker<BoneAuthoring>
    {
        GameObject getRoot(GameObject go)
        {
            Transform transf = go.transform;
            while( transf.parent )
            { 
                transf = transf.parent;
            } 
            return transf.gameObject;
        }
        public override void Bake(BoneAuthoring authoring)
        {
            var thisEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(thisEntity, new RagdollMasterData()
            {
                MasterEntity = GetEntity(authoring.MasterObjet, TransformUsageFlags.Dynamic),
            });

            AddComponent(thisEntity, new RagdollBoneData()
            { 
                m_BlendFactor = 0.0f
            });
        }
    }
}


