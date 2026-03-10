using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] float interactionDistance = 3f;
    [SerializeField] DialogueData dialogue;

    public float InteractionDistance => interactionDistance;

    public void Interact()
    {
        if (dialogue == null) return;
        DialogueManager.Instance.StartDialogue(gameObject.name, dialogue);
    }
}
