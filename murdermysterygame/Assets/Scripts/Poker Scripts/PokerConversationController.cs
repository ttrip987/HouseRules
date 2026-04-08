using UnityEngine;

public class PokerConversationController : MonoBehaviour
{
    public static PokerConversationController Instance;

    [Header("Dialogue System")]
    public Dialogue dialogue;

    [Header("Main Poker Dialogue")]
    public DialogueNodeAsset roundStartNode;
    public DialogueNodeAsset firstDrawDoneNode;
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
        if (dialogue == null)
            return;

        StartPokerDialogue(roundStartNode, true);
    }

    public void OnDrawCycleComplete(int cycleNumber)
    {
        if (dialogue == null)
            return;

        if (cycleNumber == 1)
            StartPokerDialogue(firstDrawDoneNode, false);
    }

    public void OnPlayerWin()
    {
        if (dialogue == null)
            return;

        StartPokerDialogue(playerWinNode, false);
    }

    public void OnPlayerLose()
    {
        if (dialogue == null)
            return;

        StartPokerDialogue(playerLoseNode, false);
    }

    public void OnTie()
    {
        if (dialogue == null)
            return;

        StartPokerDialogue(tieNode, false);
    }

    public void OnMatchWon()
    {
        if (dialogue == null)
            return;

        StartPokerDialogue(matchWonNode, false);
    }

    void StartPokerDialogue(DialogueNodeAsset node, bool allowAmbientFallback)
    {
        if (dialogue == null)
            return;

        if (dialogue.IsOpen)
            dialogue.ForceCloseDialogue();

        if (node != null)
        {
            dialogue.StartDialogue(node, false);
            return;
        }

        if (allowAmbientFallback)
            TryAmbientRoundStart();
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