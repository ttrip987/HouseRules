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

    private bool discardPhase = false;
    private int cardsDiscarded = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewRound();
    }

    public void StartNewRound()
    {
        CancelInvoke(nameof(StartNewRound));


        bool showedAllInMsg = false;

        if (ChipManager.Instance != null)
        {
            int pPaid = ChipManager.Instance.PlayerPayUpTo(ante);
            int dPaid = ChipManager.Instance.DealerPayUpTo(ante);

            // If somehow both have 0, nothing can happen.
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

        discardPhase = true;
        cardsDiscarded = 0;

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ResetCardPopups();
        UIManager.Instance.HideDealerCards();

        
        if (showedAllInMsg)
        {
            Invoke(nameof(ShowDiscardPrompt), 1.0f);
        }
        else
        {
            ShowDiscardPrompt();
        }
    }

    private void ShowDiscardPrompt()
    {
        UIManager.Instance.ShowResult("Select cards and press Discard");
    }

    public void PlayerDiscard()
    {
        if (!discardPhase) return;

        List<int> discardIndices = new List<int>();

        for (int i = 0; i < UIManager.Instance.playerPanel.childCount; i++)
        {
            CardView view = UIManager.Instance.playerPanel.GetChild(i).GetComponent<CardView>();
            if (view != null && view.selected)
                discardIndices.Add(i);
        }

        cardsDiscarded = discardIndices.Count;

        if (cardsDiscarded == 0)
        {
            UIManager.Instance.ShowResult("No cards selected. Press Draw to finish.");
            return;
        }

        player.Discard(discardIndices);

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ResetCardPopups();
        UIManager.Instance.HideDealerCards();

        UIManager.Instance.ShowResult("Press Draw to replace cards");
    }

    public void PlayerDraw()
    {
        if (!discardPhase) return;

        if (cardsDiscarded > 0)
            player.Draw(deck, cardsDiscarded);

        DealerAI();

        discardPhase = false;

        UIManager.Instance.RefreshHands();
        UIManager.Instance.RevealDealerCards();

        DetermineWinner();
    }

    public void DealerAI()
    {
        Dictionary<Rank, int> counts = new Dictionary<Rank, int>();

        foreach (CardData c in dealer.cards)
        {
            if (!counts.ContainsKey(c.rank))
                counts[c.rank] = 0;

            counts[c.rank]++;
        }

        List<int> discard = new List<int>();

        for (int i = 0; i < dealer.cards.Count; i++)
        {
            if (counts[dealer.cards[i].rank] == 1)
                discard.Add(i);
        }

        dealer.Discard(discard);
        dealer.Draw(deck, discard.Count);
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
        }
        else if (dealerScore > playerScore)
        {
            result = "Dealer Wins!";
            if (ChipManager.Instance != null) ChipManager.Instance.PayoutToDealer();
        }
        else
        {
            result = "Tie!";
            if (ChipManager.Instance != null) ChipManager.Instance.SplitPot();
        }

        UIManager.Instance.ShowResult(
            "Player: " + playerResult.handName +
            "\nDealer: " + dealerResult.handName +
            "\n\n" + result
        );

        UIManager.Instance.PopUpWinningCards(playerResult.winningCards, UIManager.Instance.playerPanel);
        UIManager.Instance.PopUpWinningCards(dealerResult.winningCards, UIManager.Instance.dealerPanel);

        
        if (ChipManager.Instance != null && ChipManager.Instance.IsGameOver())
            return;

        CancelInvoke(nameof(StartNewRound));
        Invoke(nameof(StartNewRound), nextRoundDelay);
    }
}