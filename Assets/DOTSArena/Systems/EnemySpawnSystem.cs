using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemySpawnSystem : ISystem
{
    private Random random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawner>();
        random = new Random(12345u);
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            spawner.ValueRW.SpawnTimer += deltaTime;    
            if (spawner.ValueRW.SpawnTimer >= spawner.ValueRO.SpawnInterval)
            {
                float3 spawnPosition = new float3(
                    random.NextFloat(-spawner.ValueRO.SpawnRadius, spawner.ValueRO.SpawnRadius),
                    0.0f,
                    random.NextFloat(-spawner.ValueRO.SpawnRadius, spawner.ValueRO.SpawnRadius)
                );

                var spawnedEnemy = commandBuffer.Instantiate(spawner.ValueRO.Prefab);
                commandBuffer.SetComponent(spawnedEnemy, LocalTransform.FromPosition(spawnPosition)); 

                // reset timer
                spawner.ValueRW.SpawnTimer -= spawner.ValueRO.SpawnInterval;
            }
        }
    }
}
