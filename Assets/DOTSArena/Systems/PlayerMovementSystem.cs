using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (localTransform, moveSpeed, playerInputState) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRO<PlayerInputState>>().WithAll<PlayerTag>())
        {
            float3 direction = new float3(playerInputState.ValueRO.Move.x, 0f, playerInputState.ValueRO.Move.y);

            if (math.lengthsq(direction) > 1f)
            {
                direction = math.normalize(direction);
            }

            localTransform.ValueRW.Position += direction * moveSpeed.ValueRO.Value * deltaTime;
        }
    }
}
