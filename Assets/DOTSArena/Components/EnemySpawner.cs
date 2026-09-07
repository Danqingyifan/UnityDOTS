using Unity.Entities;

public struct EnemySpawner : IComponentData
{
    public Entity Prefab;
    public float SpawnInterval;
    public float SpawnTimer;
    public float SpawnRadius;
}
