using Unity.Entities;
using UnityEngine;

public class RagdollBodyAuthoring : MonoBehaviour
{
   class Baker : Baker<RagdollBodyAuthoring>
   {
        public override void Bake(RagdollBodyAuthoring authoring)
        {
            var thisEntity = GetEntity(TransformUsageFlags.Dynamic);

            //UnityEngine.Collider[] colliders = authoring.GetComponentsInChildren<UnityEngine.Collider>();
            //for (int i = 0; i < colliders.Length; i++)
            //{
            //    var partE = GetEntity(colliders[i].gameObject, TransformUsageFlags.Dynamic);

            //    AddComponent(partE, new RagdollBoneData()
            //    {
            //        m_BlendFactor = 0.0f
            //    });
            //}
        }
    }

}


