using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawPokerGameManager : MonoBehaviour
{
    public static DrawPokerGameManager Instance;
    public Deck deck;
    public int fixedBet = 100;
    public PlayerHand player;
    public PlayerHand dealer;
    public bool canDiscard = true;
    public bool canDraw = false;

    void Awake() { Instance = this; }

    void Start() { StartNewRound(); }

    public void StartNewRound()
    {
        if (ChipManager.Instance.IsGameOver()) return;
        deck.ResetDeck();
        player = new PlayerHand();
        dealer = new PlayerHand();
        player.Draw(deck, 5);
        dealer.Draw(deck, 5);
        UIManager.Instance.RefreshHands();
        ResetAllCards();
        ChipManager.Instance.PlayerSpend(fixedBet);
        ChipManager.Instance.DealerSpend(fixedBet);
        canDiscard = true;
        canDraw = false;
        UIManager.Instance.ShowResult($"Each player bets {fixedBet} chips.\nSelect cards to discard.");
    }

    public void PlayerDiscard(List<int> discardIndices)
    {
        if (!canDiscard) return;
        player.Discard(discardIndices);
        player.Draw(deck, discardIndices.Count);
        canDiscard = false;
        canDraw = true;
        UIManager.Instance.RefreshHands();
        UIManager.Instance.ShowResult("Press Draw to complete the round.");
    }

    public void PlayerDraw()
    {
        if (!canDraw) return;
        DealerAI();
        canDraw = false;
        DetermineWinner();
        UIManager.Instance.RefreshHands();
    }

    public void DealerAI()
    {
        Dictionary<Rank, int> counts = new Dictionary<Rank, int>();
        foreach (var c in dealer.cards)
        {
            if (!counts.ContainsKey(c.rank)) counts[c.rank] = 0;
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
        (long playerScore, List<CardData> playerWinningCards, string playerHandName) = HandEvaluator.EvaluateHandWithCards(player.cards);
        (long dealerScore, List<CardData> dealerWinningCards, string dealerHandName) = HandEvaluator.EvaluateHandWithCards(dealer.cards);

        PlayerHand winnerHand = null;
        List<CardData> winningCards = null;
        string resultText;

        if (playerScore > dealerScore)
        {
            ChipManager.Instance.PayoutToPlayer();
            winnerHand = player;
            winningCards = playerWinningCards;
            resultText = $"Player wins!\nPlayer Hand: {playerHandName}\nDealer Hand: {dealerHandName}";
        }
        else if (dealerScore > playerScore)
        {
            ChipManager.Instance.PayoutToDealer();
            winnerHand = dealer;
            winningCards = dealerWinningCards;
            resultText = $"Dealer wins!\nPlayer Hand: {playerHandName}\nDealer Hand: {dealerHandName}";
        }
        else
        {
            int splitPot = ChipManager.Instance.pot / 2;
            ChipManager.Instance.playerChips += splitPot;
            ChipManager.Instance.dealerChips += splitPot;
            ChipManager.Instance.pot = 0;
            ChipManager.Instance.UpdateUI();
            resultText = $"Tie!\nPlayer Hand: {playerHandName}\nDealer Hand: {dealerHandName}";
        }

        UIManager.Instance.ShowResult(resultText);
        HighlightWinningCards(winnerHand, winningCards);

        if (!ChipManager.Instance.IsGameOver())
            Invoke(nameof(PrepareNextRound), 3f);
    }

    void HighlightWinningCards(PlayerHand winningHand, List<CardData> winningCards)
    {
        ResetAllCards();
        if (winningHand == null || winningCards == null) return;
        Transform panel = winningHand == player ? UIManager.Instance.playerPanel : UIManager.Instance.dealerPanel;
        for (int i = 0; i < panel.childCount; i++)
        {
            CardView card = panel.GetChild(i).GetComponent<CardView>();
            if (card != null && winningCards.Contains(card.data))
                card.PopUp(true);
        }
    }

    void ResetAllCards()
    {
        foreach (Transform child in UIManager.Instance.playerPanel)
        {
            CardView card = child.GetComponent<CardView>();
            if (card != null) card.PopUp(false);
        }
        foreach (Transform child in UIManager.Instance.dealerPanel)
        {
            CardView card = child.GetComponent<CardView>();
            if (card != null) card.PopUp(false);
        }
    }

    public void PlayerDiscard()
    {
        List<int> discardIndices = new List<int>();

        for (int i = 0; i < UIManager.Instance.playerPanel.childCount; i++)
        {
            CardView card = UIManager.Instance.playerPanel.GetChild(i).GetComponent<CardView>();
            if (card != null && card.selected)
                discardIndices.Add(i);
        }

        if (discardIndices.Count > 0)
        {
            DrawPokerGameManager.Instance.player.Discard(discardIndices);
            DrawPokerGameManager.Instance.player.Draw(DrawPokerGameManager.Instance.deck, discardIndices.Count);

            DrawPokerGameManager.Instance.DealerAI(); 
            DrawPokerGameManager.Instance.DetermineWinner();

            UIManager.Instance.RefreshHands();
            UIManager.Instance.ResetCardPopups();
        }
    }

    void PrepareNextRound()
    {
        ResetAllCards();
        DrawPokerGameManager.Instance.StartNewRound();
    }
}