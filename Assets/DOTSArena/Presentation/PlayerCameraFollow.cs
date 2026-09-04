using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 10f, -10f);

    private World world;
    private EntityManager entityManager;
    private EntityQuery playerQuery;
    private bool playerQueryCreated;

    private void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;

        if (world == null || !world.IsCreated)
        {
            Debug.LogError("PlayerCameraFollow could not find the default ECS World.", this);
            enabled = false;
            return;
        }

        entityManager = world.EntityManager;
        playerQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<LocalTransform>());
        playerQueryCreated = true;
    }

    private void LateUpdate()
    {
        if (!playerQueryCreated || playerQuery.CalculateEntityCount() != 1)
        {
            return;
        }

        var playerEntity = playerQuery.GetSingletonEntity();
        var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        var playerPosition = playerTransform.Position;

        transform.position = new Vector3(
            playerPosition.x,
            playerPosition.y,
            playerPosition.z) + offset;
    }

    private void OnDestroy()
    {
        if (playerQueryCreated && world != null && world.IsCreated)
        {
            playerQuery.Dispose();
        }
    }
}
