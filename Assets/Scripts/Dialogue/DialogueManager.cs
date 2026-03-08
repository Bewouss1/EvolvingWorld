using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    static DialogueManager instance;
    public static DialogueManager Instance => instance;

    [Header("UI References")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI npcNameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Transform choicesContainer;
    [SerializeField] GameObject choiceButtonPrefab;

    DialogueData currentDialogue;
    string currentNpcName;

    public bool IsOpen => dialoguePanel.activeSelf;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcName, DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNpcName = npcName;
        dialoguePanel.SetActive(true);
        ShowNode(0);
    }

    void ShowNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= currentDialogue.nodes.Length)
        {
            CloseDialogue();
            return;
        }

        DialogueNode node = currentDialogue.nodes[nodeIndex];

        npcNameText.text = currentNpcName;
        dialogueText.text = node.npcText;

        ClearChoices();

        foreach (DialogueChoice choice in node.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            int nextIndex = choice.nextNodeIndex;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(nextIndex));
        }
    }

    void OnChoiceSelected(int nextNodeIndex)
    {
        if (nextNodeIndex < 0)
            CloseDialogue();
        else
            ShowNode(nextNodeIndex);
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        ClearChoices();
        currentDialogue = null;
    }
}
