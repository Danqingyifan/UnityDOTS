using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Min(0.0f)]
    public float Speed = 1.5f;

    [Min(0.1f)]
    public float Lifetime = 30.0f;

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);
            AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
            AddComponent(entity, new Lifetime { Remaining = authoring.Lifetime });
        }
    }
}
