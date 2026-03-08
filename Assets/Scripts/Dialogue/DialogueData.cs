using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] nodes;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(2, 5)]
    public string npcText;
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex; // -1 = fermer le dialogue
}
