using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;

    [Min(0.1f)]
    public float SpawnInterval = 2.0f;

    [Min(0.0f)]
    public float SpawnRadius = 5.0f;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemySpawner
            {
                Prefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.SpawnInterval,
                SpawnTimer = 0.0f,   
                SpawnRadius = authoring.SpawnRadius
            });
        }
    }
}
