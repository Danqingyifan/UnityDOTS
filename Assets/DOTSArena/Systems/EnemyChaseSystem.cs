using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerMovementSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemyChaseSystem : ISystem
{
    private EntityQuery playerQuery;

    public void OnCreate(ref SystemState state)
    {
        playerQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlayerTag>(),ComponentType.ReadOnly<LocalTransform>());

        state.RequireForUpdate(playerQuery);
        state.RequireForUpdate<EnemyTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (playerQuery.CalculateEntityCount() != 1)
        {
            return;
        }

        // Singleton reads do not automatically complete component dependencies.
        playerQuery.CompleteDependency();

        float3 playerPosition = playerQuery.GetSingleton<LocalTransform>().Position;
        state.Dependency = new EnemyChaseJob
        {
            PlayerPosition = playerPosition,
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel(state.Dependency);
    }
}


[BurstCompile]
[WithAll(typeof(EnemyTag))]
public partial struct EnemyChaseJob : IJobEntity
{
    private const float StopDistance = 1f;

    public float3 PlayerPosition;
    public float DeltaTime;

    public void Execute(ref LocalTransform transform, in MoveSpeed moveSpeed)
    {
        float3 toPlayer = PlayerPosition - transform.Position;
        toPlayer.y = 0f;

        float distance = math.length(toPlayer);
        if (distance <= StopDistance)
        {
            return;
        }

        // Stop at the boundary even if this frame's step would overshoot it.
        float step = math.min(
            math.max(0f, moveSpeed.Value * DeltaTime),
            distance - StopDistance);
        transform.Position += toPlayer * (step / distance);
    }
}
