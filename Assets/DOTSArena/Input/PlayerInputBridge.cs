using UnityEngine;
using UnityEngine.InputSystem;

using Unity.Entities;
using Unity.Mathematics;

public class PlayerInputBridge : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;

    private World world;
    private EntityManager entityManager;
    private EntityQuery inputQuery;
    private bool inputQueryCreated;

    private void OnEnable()
    {
        if (moveAction == null)
        {
            Debug.LogError("PlayerInputBridge requires a Move Action reference.", this);
            enabled = false;
            return;
        }

        moveAction.action.Enable();
    }

    private void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;

        if (world == null || !world.IsCreated)
        {
            Debug.LogError("PlayerInputBridge could not find the default ECS World.", this);
            enabled = false;
            return;
        }

        entityManager = world.EntityManager;
        inputQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PlayerInputState>());
        inputQueryCreated = true;
    }

    private void Update()
    {
        if (!inputQueryCreated || inputQuery.CalculateEntityCount() != 1)
        {
            return;
        }

        if (moveAction == null || !moveAction.action.enabled)
        {
            return;
        }

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        PlayerInputState inputState = new PlayerInputState { Move = new float2(moveInput.x, moveInput.y) };

        entityManager.SetComponentData(inputQuery.GetSingletonEntity(), inputState);
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void OnDestroy()
    {
        if (inputQueryCreated && world != null && world.IsCreated)
        {
            inputQuery.Dispose();
        }
    }
}
