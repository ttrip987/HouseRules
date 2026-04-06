using UnityEngine;

public class PokerConversationController : MonoBehaviour
{
    public static PokerConversationController Instance;

    [Header("Dialogue System")]
    public Dialogue dialogue;

    [Header("Main Poker Dialogue")]
    public DialogueNodeAsset roundStartNode;
    public DialogueNodeAsset playerWinNode;
    public DialogueNodeAsset playerLoseNode;
    public DialogueNodeAsset tieNode;
    public DialogueNodeAsset matchWonNode;

    [Header("Ambient Dialogue")]
    public DialogueNodeAsset[] ambientRoundStartNodes;
    [Range(0f, 1f)] public float ambientChance = 0.35f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (dialogue == null)
            dialogue = FindObjectOfType<Dialogue>();
    }

    public void OnRoundStart()
    {
        if (dialogue == null || dialogue.IsOpen)
            return;

        if (roundStartNode != null)
        {
            dialogue.StartDialogue(roundStartNode, false);
            return;
        }

        TryAmbientRoundStart();
    }

    public void OnPlayerWin()
    {
        if (dialogue == null || dialogue.IsOpen)
            return;

        if (playerWinNode != null)
            dialogue.StartDialogue(playerWinNode, false);
    }

    public void OnPlayerLose()
    {
        if (dialogue == null || dialogue.IsOpen)
            return;

        if (playerLoseNode != null)
            dialogue.StartDialogue(playerLoseNode, false);
    }

    public void OnTie()
    {
        if (dialogue == null || dialogue.IsOpen)
            return;

        if (tieNode != null)
            dialogue.StartDialogue(tieNode, false);
    }

    public void OnMatchWon()
    {
        if (dialogue == null || dialogue.IsOpen)
            return;

        if (matchWonNode != null)
            dialogue.StartDialogue(matchWonNode, false);
    }

    void TryAmbientRoundStart()
    {
        if (ambientRoundStartNodes == null || ambientRoundStartNodes.Length == 0)
            return;

        if (Random.value > ambientChance)
            return;

        int index = Random.Range(0, ambientRoundStartNodes.Length);
        DialogueNodeAsset node = ambientRoundStartNodes[index];

        if (node != null)
            dialogue.StartDialogue(node, false);
    }
}