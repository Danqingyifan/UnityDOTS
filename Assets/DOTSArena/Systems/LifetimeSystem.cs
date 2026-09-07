using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemySpawnSystem))]
public partial struct LifetimeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Lifetime>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var parallelWriter = commandBuffer.AsParallelWriter();
        var lifetimeJob = new LifetimeJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            CommandBuffer = parallelWriter
        };
        state.Dependency = lifetimeJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct LifetimeJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(
        [ChunkIndexInQuery] int sortKey,
        Entity entity,
        ref Lifetime lifetime,
        in EnemyTag enemyTag)
    {
        lifetime.Remaining -= DeltaTime;
        if (lifetime.Remaining <= 0)
        {
            CommandBuffer.DestroyEntity(sortKey,entity);
        }
    }
}


