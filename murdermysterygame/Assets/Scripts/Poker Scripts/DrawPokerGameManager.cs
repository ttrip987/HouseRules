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

    [Header("Discard/Draw cycles per round")]
    public int maxCycles = 2;

    private int currentCycle = 0;

    private bool discardPhase = false;

    public int winCreditReward = 25;
    public int bigWinCreditReward = 100;

    // Stores last chosen indices so Draw knows what to replace
    private List<int> lastDiscardIndices = new List<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewRound();
    }

    public bool CanSelectDiscards() => discardPhase;

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

        currentCycle = 0;
        discardPhase = true;
        lastDiscardIndices.Clear();

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ResetCardPopups();
        UIManager.Instance.HideDealerCards();
        UIManager.Instance.ClearPlayerSelections();

        if (showedAllInMsg) Invoke(nameof(ShowDiscardPrompt), 1.0f);
        else ShowDiscardPrompt();
    }

    private void ShowDiscardPrompt()
    {
        UIManager.Instance.ShowResult($"Cycle {currentCycle + 1}/{maxCycles}: Select cards and press Discard");
    }

    // Button: Discard
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

        discardIndices.Sort();
        lastDiscardIndices = discardIndices;

        if (lastDiscardIndices.Count == 0)
        {
            UIManager.Instance.ShowResult("No cards selected. Press Draw to continue.");
            return;
        }

        UIManager.Instance.ShowResult("Press Draw to replace cards");
    }

    // Button: Draw
    public void PlayerDraw()
    {
        if (!discardPhase) return;
        StartCoroutine(PlayerCycleRoutine());
    }

    private IEnumerator PlayerCycleRoutine()
    {
        // PLAYER discard/draw (with animation)
        if (lastDiscardIndices.Count > 0)
        {
            // 1) animate discards
            yield return UIManager.Instance.AnimateDiscard(
                UIManager.Instance.playerPanel,
                lastDiscardIndices,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);

            // 2) apply data change
            player.Discard(lastDiscardIndices);
            player.Draw(deck, lastDiscardIndices.Count);

            // 3) refresh to show new cards in their slots
            UIManager.Instance.RefreshHands();
            UIManager.Instance.HideDealerCards();

            // 4) animate draws (new cards fly in)
            yield return UIManager.Instance.AnimateDraw(
                UIManager.Instance.playerPanel,
                lastDiscardIndices,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);

            UIManager.Instance.ClearPlayerSelections();
        }

        // DEALER cycle (AI + animation)
        yield return StartCoroutine(DealerCycleRoutine());

        // next cycle or showdown
        currentCycle++;
        lastDiscardIndices.Clear();

        if (currentCycle < maxCycles)
        {
            UIManager.Instance.ResetCardPopups();
            UIManager.Instance.HideDealerCards();
            UIManager.Instance.ClearPlayerSelections();
            ShowDiscardPrompt();
        }
        else
        {
            discardPhase = false;

            UIManager.Instance.RefreshHands();
            UIManager.Instance.RevealDealerCards();
            DetermineWinner();
        }
    }

    private IEnumerator DealerCycleRoutine()
    {
        List<int> discard = DealerPickDiscards();

        if (discard.Count > 0)
        {
            yield return UIManager.Instance.AnimateDiscard(
                UIManager.Instance.dealerPanel,
                discard,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);
        }

        if (discard.Count > 0)
        {
            dealer.Discard(discard);
            dealer.Draw(deck, discard.Count);

            UIManager.Instance.RefreshHands();
            UIManager.Instance.HideDealerCards();

            yield return UIManager.Instance.AnimateDraw(
                UIManager.Instance.dealerPanel,
                discard,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);
        }
        else
        {
            UIManager.Instance.RefreshHands();
            UIManager.Instance.HideDealerCards();
        }
    }

    private List<int> DealerPickDiscards()
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

        discard.Sort();
        return discard;
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