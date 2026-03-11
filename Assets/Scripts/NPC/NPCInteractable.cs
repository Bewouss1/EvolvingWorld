using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] float interactionDistance = 3f;
    [SerializeField] DialogueData dialogue;
    [SerializeField] GameObject defaultModel;

    [Header("Réactions aux événements monde")]
    [SerializeField] EventDialogue[] eventDialogues;

    public float InteractionDistance => interactionDistance;

    void OnEnable()
    {
        if (WorldEventManager.Instance != null)
            WorldEventManager.Instance.OnEventsChanged += UpdateAppearance;
    }

    void OnDisable()
    {
        if (WorldEventManager.Instance != null)
            WorldEventManager.Instance.OnEventsChanged -= UpdateAppearance;
    }

    public void Interact()
    {
        DialogueData chosen = GetCurrentDialogue();
        if (chosen == null) return;
        DialogueManager.Instance.StartDialogue(gameObject.name, chosen);
    }

    // Retourne le dialogue correspondant à la phase du premier événement actif/warning
    DialogueData GetCurrentDialogue()
    {
        if (eventDialogues != null && WorldEventManager.Instance != null)
        {
            foreach (var ed in eventDialogues)
            {
                if (ed.worldEvent == null) continue;

                if (WorldEventManager.Instance.IsActive(ed.worldEvent) && ed.activeDialogue != null)
                    return ed.activeDialogue;

                if (WorldEventManager.Instance.IsWarning(ed.worldEvent) && ed.warningDialogue != null)
                    return ed.warningDialogue;
            }
        }
        return dialogue;
    }

    // Swap le modèle du PNJ selon la phase de l'événement
    void UpdateAppearance()
    {
        // Désactiver tous les modèles alternatifs
        if (eventDialogues != null)
        {
            foreach (var ed in eventDialogues)
            {
                if (ed.warningModel != null) ed.warningModel.SetActive(false);
                if (ed.activeModel != null) ed.activeModel.SetActive(false);
            }
        }

        // Trouver le premier événement qui matche
        GameObject modelToShow = null;
        if (eventDialogues != null && WorldEventManager.Instance != null)
        {
            foreach (var ed in eventDialogues)
            {
                if (ed.worldEvent == null) continue;

                if (WorldEventManager.Instance.IsActive(ed.worldEvent) && ed.activeModel != null)
                {
                    modelToShow = ed.activeModel;
                    break;
                }

                if (WorldEventManager.Instance.IsWarning(ed.worldEvent) && ed.warningModel != null)
                {
                    modelToShow = ed.warningModel;
                    break;
                }
            }
        }

        // Afficher le bon modèle
        if (defaultModel != null)
            defaultModel.SetActive(modelToShow == null);

        if (modelToShow != null)
            modelToShow.SetActive(true);
    }
}

[System.Serializable]
public class EventDialogue
{
    public WorldEventData worldEvent;
    public DialogueData warningDialogue;
    public DialogueData activeDialogue;
    [Tooltip("Modèle alternatif phase avertissement (optionnel)")]
    public GameObject warningModel;
    [Tooltip("Modèle alternatif phase active (optionnel)")]
    public GameObject activeModel;
}
