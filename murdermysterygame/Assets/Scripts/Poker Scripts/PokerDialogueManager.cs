using UnityEngine;

public class PokerDialogueManager : MonoBehaviour
{
    public static PokerDialogueManager Instance;

    [Header("UI")]
    public PokerDialogueUI dialogueUI;

    private int attitude = 0;

    void Awake()
    {
        Instance = this;
    }
 
    public void ShowRoundStartDialogue()
    {
        if (dialogueUI == null)
            return;

        dialogueUI.ShowChoices(
            "Dealer: Cards are out. What do you say?",
            new string[]
            {
                "Bluff",
                "Stay quiet",
                "Ask about them"
            },
            new System.Action[]
            {
                Bluff,
                StayQuiet,
                AskAboutThem
            }
        );
    }

    void Bluff()
    {
        attitude -= 1;

        dialogueUI.ShowMessage(
            "You: I've got this one.\nDealer: Confident. We'll see.",
            FinishChoice
        );
    }

    void StayQuiet()
    {
        dialogueUI.ShowMessage(
            "You stay quiet.\nDealer: Silent type, huh?",
            FinishChoice
        );
    }

    void AskAboutThem()
    {
        attitude += 1;

        dialogueUI.ShowMessage(
            "You: You play here a lot?\nDealer: More than I should.",
            FinishChoice
        );
    }

    void FinishChoice()
    {
      
    }

    public void ShowWinDialogue(string playerHandName, string dealerHandName)
    {
        if (dialogueUI == null)
            return;

        dialogueUI.ShowMessage(
            "Dealer: Damn. You won that one.\n" +
            "You had " + playerHandName + "."
        );
    }

    public void ShowLoseDialogue(string playerHandName, string dealerHandName)
    {
        if (dialogueUI == null)
            return;

        dialogueUI.ShowMessage(
            "Dealer: House takes it.\n" +
            "I had " + dealerHandName + "."
        );
    }

    public void ShowTieDialogue(string playerHandName, string dealerHandName)
    {
        if (dialogueUI == null)
            return;

        dialogueUI.ShowMessage(
            "Dealer: A tie, huh?\n" +
            "We both matched pretty closely."
        );
    }

}