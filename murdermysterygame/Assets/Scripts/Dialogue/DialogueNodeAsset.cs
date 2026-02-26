using UnityEngine;

public enum DialogueAction
{
    None,
    LoadPokerScene
}

[CreateAssetMenu(menuName = "Dialogue/Node")]
public class DialogueNodeAsset : ScriptableObject
{
    [TextArea(5, 20)]
    public string line;

    public string speakerName;

    public DialogueChoiceAsset[] choices;

    public DialogueNodeAsset nextNode;
}

[System.Serializable]
public class DialogueChoiceAsset
{
    public string choiceText;
    public DialogueNodeAsset nextNode;

    public DialogueAction action;  // ✅ NEW
}