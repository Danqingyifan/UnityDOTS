using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour
{
    [Min(0.0f)]
    public float Speed = 5.0f; // Player movement speed

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new MoveSpeed { Value = authoring.Speed });
        }
    }

}
