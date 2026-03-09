using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private LayerMask interactableLayers;

    [Header("UI")]
    [SerializeField] private InventoryUI inventoryUI;

    float lookRotationSpeed = 8f;

    CustomActions input;
    NavMeshAgent agent;

    // Interaction
    MonoBehaviour currentInteractable;
    Transform interactableTarget;

    // === Lifecycle ===

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new CustomActions();
        AssignInputs();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        FaceMovementDirection();
        CheckInteractableReached();
    }

    // === Input ===

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen) return;
        if (inventoryUI != null && inventoryUI.IsOpen) return;

        RaycastHit hit;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out hit, Mathf.Infinity))
            return;

        int hitLayer = 1 << hit.collider.gameObject.layer;

        if ((hitLayer & interactableLayers) != 0)
        {
            currentInteractable = hit.collider.GetComponent<NPCInteractable>();
            interactableTarget = hit.collider.transform;
            agent.destination = interactableTarget.position;
            return;
        }

        ClearInteractable();

        if ((hitLayer & clickableLayers) != 0)
        {
            agent.destination = hit.point;
            if (clickEffect != null)
            {
                GameObject effect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation).gameObject;
                Destroy(effect, clickEffect.main.duration);
            }
        }
    }

    // === Movement ===

    public void MoveTo(Vector3 position)
    {
        agent.destination = position;
    }

    public void Stop()
    {
        agent.ResetPath();
    }

    void FaceMovementDirection()
    {
        if (agent.velocity.sqrMagnitude < 0.1f) return;
        Vector3 flatDirection = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    // === Interaction ===

    void CheckInteractableReached()
    {
        if (currentInteractable == null) return;

        float distance = Vector3.Distance(transform.position, interactableTarget.position);
        NPCInteractable npc = currentInteractable as NPCInteractable;
        if (npc != null && distance <= npc.InteractionDistance)
        {
            Stop();
            npc.Interact();
            ClearInteractable();
        }
    }

    void ClearInteractable()
    {
        currentInteractable = null;
        interactableTarget = null;
    }
}
