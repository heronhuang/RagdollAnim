using Unity.Entities;
using UnityEngine;

public class FireAuthoring : MonoBehaviour
{
    public GameObject mBulletTemplate;
    public float SHOT_COOLDOWN = 0.1F;
    public float BULLET_SPEED = 100.0f;
    public float BULLET_FORWARD_OFFSET = 1.0f;
    public float BULLET_UP_OFFSET = 1.0f;

    class Baker : Baker<FireAuthoring>
    {
        public override void Bake(FireAuthoring authoring)
        {
            var thisEntity = GetEntity(TransformUsageFlags.Dynamic);
            var mBulletTemplate = GetEntity(authoring.mBulletTemplate, TransformUsageFlags.Dynamic);
            AddComponent(thisEntity, new FireData()
            {
                mBulletTemplate = mBulletTemplate,
                SHOT_COOLDOWN = authoring.SHOT_COOLDOWN,
                BULLET_SPEED = authoring.BULLET_SPEED,
                BULLET_FORWARD_OFFSET = authoring.BULLET_FORWARD_OFFSET,
                BULLET_UP_OFFSET = authoring.BULLET_UP_OFFSET,
            });
        }
    }
}
public struct FireData : IComponentData
{
    public Entity mBulletTemplate;
    public float SHOT_COOLDOWN;
    public float BULLET_SPEED;
    public float BULLET_FORWARD_OFFSET;
    public float BULLET_UP_OFFSET;
}

