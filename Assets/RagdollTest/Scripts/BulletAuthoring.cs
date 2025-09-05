using Unity.Entities;
using UnityEngine;
public struct BulletData : IComponentData
{ 
}

class BulletAuthoring : MonoBehaviour
{
    class BulletAuthoringBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BulletData>(entity);
        }
    }
}


