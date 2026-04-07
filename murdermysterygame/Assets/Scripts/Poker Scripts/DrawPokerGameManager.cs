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

    [Header("Draw cycles per round")]
    public int maxCycles = 2;

    private int currentCycle = 0;
    private bool drawPhase = false;

    public int winCreditReward = 25;
    public int bigWinCreditReward = 100;

    private List<int> lastDrawIndices = new List<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewRound();
    }

    public bool CanSelectDiscards() => drawPhase;

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
        drawPhase = true;
        lastDrawIndices.Clear();

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ResetCardPopups();
        UIManager.Instance.HideDealerCards();
        UIManager.Instance.ClearPlayerSelections();

        if (PokerConversationController.Instance != null)
            PokerConversationController.Instance.OnRoundStart();

        if (showedAllInMsg)
            Invoke(nameof(ShowDrawPrompt), 1.0f);
        else
            ShowDrawPrompt();
    }

    private void ShowDrawPrompt()
    {
        UIManager.Instance.ShowResult($"  ");
    }

    public void PlayerDraw()
    {
        if (!drawPhase) return;

        List<int> drawIndices = new List<int>();

        for (int i = 0; i < UIManager.Instance.playerPanel.childCount; i++)
        {
            CardView view = UIManager.Instance.playerPanel.GetChild(i).GetComponent<CardView>();
            if (view != null && view.selected)
                drawIndices.Add(i);
        }

        drawIndices.Sort();
        lastDrawIndices = drawIndices;

        StartCoroutine(PlayerCycleRoutine());
    }

    private IEnumerator PlayerCycleRoutine()
    {
        if (lastDrawIndices.Count > 0)
        {
            yield return UIManager.Instance.AnimateDiscard(
                UIManager.Instance.playerPanel,
                lastDrawIndices,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);

            player.Discard(lastDrawIndices);
            player.Draw(deck, lastDrawIndices.Count);

            UIManager.Instance.RefreshHands();
            UIManager.Instance.HideDealerCards();

            yield return UIManager.Instance.AnimateDraw(
                UIManager.Instance.playerPanel,
                lastDrawIndices,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);
        }

        UIManager.Instance.ClearPlayerSelections();

        yield return StartCoroutine(DealerCycleRoutine());

        currentCycle++;
        lastDrawIndices.Clear();

        if (currentCycle < maxCycles)
        {
            UIManager.Instance.ResetCardPopups();
            UIManager.Instance.HideDealerCards();
            UIManager.Instance.ClearPlayerSelections();

            if (PokerConversationController.Instance != null)
                PokerConversationController.Instance.OnDrawCycleComplete(currentCycle);

            ShowDrawPrompt();
        }
        else
        {
            drawPhase = false;

            UIManager.Instance.RefreshHands();
            UIManager.Instance.RevealDealerCards();
            DetermineWinner();
        }
    }

    private IEnumerator DealerCycleRoutine()
    {
        List<int> drawIndices = DealerPickDiscards();

        if (drawIndices.Count > 0)
        {
            yield return UIManager.Instance.AnimateDiscard(
                UIManager.Instance.dealerPanel,
                drawIndices,
                UIManager.Instance.flyDuration,
                UIManager.Instance.stagger);

            dealer.Discard(drawIndices);
            dealer.Draw(deck, drawIndices.Count);

            UIManager.Instance.RefreshHands();
            UIManager.Instance.HideDealerCards();

            yield return UIManager.Instance.AnimateDraw(
                UIManager.Instance.dealerPanel,
                drawIndices,
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

            if (PokerConversationController.Instance != null)
                PokerConversationController.Instance.OnPlayerWin();
        }
        else if (dealerScore > playerScore)
        {
            result = "Dealer Wins!";
            if (ChipManager.Instance != null) ChipManager.Instance.PayoutToDealer();

            if (PokerConversationController.Instance != null)
                PokerConversationController.Instance.OnPlayerLose();
        }
        else
        {
            result = "Tie!";
            if (ChipManager.Instance != null) ChipManager.Instance.SplitPot();

            if (PokerConversationController.Instance != null)
                PokerConversationController.Instance.OnTie();
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

            if (PokerConversationController.Instance != null)
                PokerConversationController.Instance.OnMatchWon();

            Debug.Log("Player beat the dealer and earned permanent credits!");
        }
    }
}