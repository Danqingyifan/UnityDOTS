using Unity.Entities;
using Unity.Transforms;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (localTransform, moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>().WithAll<PlayerTag>())
        {
            localTransform.ValueRW.Position.x += moveSpeed.ValueRO.Value * deltaTime;
        }
    }
}