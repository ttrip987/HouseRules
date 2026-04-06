using UnityEngine;

public class PokerDialogueTrigger : MonoBehaviour
{
    public DialogueNodeAsset nodeToStart;

    public void TriggerDialogue()
    {
        Dialogue dialogue = FindObjectOfType<Dialogue>();

        if (dialogue != null && nodeToStart != null)
            dialogue.StartDialogue(nodeToStart);
    }
}