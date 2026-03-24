using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPokerGameManager : MonoBehaviour
{
    public static DrawPokerGameManager Instance;

    public Deck deck;
    public PlayerHand player;
    public PlayerHand dealer;

    public int ante = 100;
    public float nextRoundDelay = 2.0f;
    public float revealDelay = 1.0f;

    public int winCreditReward = 25;
    public int bigWinCreditReward = 100;

    private bool roundInProgress = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewRound();
    }

    public bool CanSelectDiscards()
    {
        return false;
    }

    public void StartNewRound()
    {
        CancelInvoke(nameof(StartNewRound));

        bool showedAllInMsg = false;

        if (ChipManager.Instance != null)
        {
            int pPaid = ChipManager.Instance.PlayerPayUpTo(ante);
            int dPaid = ChipManager.Instance.DealerPayUpTo(ante);

            if (pPaid == 0 && dPaid == 0)
            {
                ChipManager.Instance.IsGameOver();
                return;
            }

            if (pPaid < ante && dPaid < ante)
            {
                UIManager.Instance.ShowResult("Both players are ALL-IN on the ante!");
                showedAllInMsg = true;
            }
            else if (pPaid < ante)
            {
                UIManager.Instance.ShowResult("Player is ALL-IN on the ante!");
                showedAllInMsg = true;
            }
            else if (dPaid < ante)
            {
                UIManager.Instance.ShowResult("Wesley is ALL-IN on the ante!");
                showedAllInMsg = true;
            }
        }

        deck.ResetDeck();

        player = new PlayerHand();
        dealer = new PlayerHand();

        player.Draw(deck, 5);
        dealer.Draw(deck, 5);

        roundInProgress = true;

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ResetCardPopups();
        UIManager.Instance.HideDealerCards();
        UIManager.Instance.ClearPlayerSelections();

        if (PokerDialogueManager.Instance != null)
        {
            PokerDialogueManager.Instance.ShowRoundStartDialogue();
        }
        else
        {
            if (showedAllInMsg)
                Invoke(nameof(RevealAndScoreRound), 1.0f);
            else
                Invoke(nameof(RevealAndScoreRound), revealDelay);
        }
    }

    public void ContinueFromDialogue()
    {
        if (!roundInProgress)
            return;

        CancelInvoke(nameof(RevealAndScoreRound));
        Invoke(nameof(RevealAndScoreRound), revealDelay);
    }

    private void RevealAndScoreRound()
    {
        roundInProgress = false;

        UIManager.Instance.RefreshHands();
        UIManager.Instance.RevealDealerCards();

        DetermineWinner();
    }

    public void DetermineWinner()
    {
        var playerResult = HandEvaluator.EvaluateHandWithCards(player.cards);
        var dealerResult = HandEvaluator.EvaluateHandWithCards(dealer.cards);

        long playerScore = playerResult.score;
        long dealerScore = dealerResult.score;

        string result;

        if (playerScore > dealerScore)
        {
            result = "Player Wins!";
            if (ChipManager.Instance != null) ChipManager.Instance.PayoutToPlayer();

            if (PokerDialogueManager.Instance != null)
                PokerDialogueManager.Instance.ShowWinDialogue(playerResult.handName, dealerResult.handName);
        }
        else if (dealerScore > playerScore)
        {
            result = "Dealer Wins!";
            if (ChipManager.Instance != null) ChipManager.Instance.PayoutToDealer();

            if (PokerDialogueManager.Instance != null)
                PokerDialogueManager.Instance.ShowLoseDialogue(playerResult.handName, dealerResult.handName);
        }
        else
        {
            result = "Tie!";
            if (ChipManager.Instance != null) ChipManager.Instance.SplitPot();

            if (PokerDialogueManager.Instance != null)
                PokerDialogueManager.Instance.ShowTieDialogue(playerResult.handName, dealerResult.handName);
        }

        UIManager.Instance.ShowResult(
            "Player: " + playerResult.handName +
            "\nDealer: " + dealerResult.handName +
            "\n\n" + result
        );

        UIManager.Instance.PopUpWinningCards(playerResult.winningCards, UIManager.Instance.playerPanel);
        UIManager.Instance.PopUpWinningCards(dealerResult.winningCards, UIManager.Instance.dealerPanel);

        CheckForMatchEnd();

        if (ChipManager.Instance != null && ChipManager.Instance.IsGameOver())
            return;

        CancelInvoke(nameof(StartNewRound));
        Invoke(nameof(StartNewRound), nextRoundDelay);
    }

    void CheckForMatchEnd()
    {
        if (ChipManager.Instance == null)
            return;

        if (ChipManager.Instance.dealerChips <= 0)
        {
            if (CreditManager.Instance != null)
                CreditManager.Instance.AddCredits(bigWinCreditReward);

            Debug.Log("Player beat the dealer and earned permanent credits!");
        }
    }
}