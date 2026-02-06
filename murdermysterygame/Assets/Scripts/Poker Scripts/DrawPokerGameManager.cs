using UnityEngine;
using System.Collections.Generic;

public class DrawPokerGameManager : MonoBehaviour
{
    public static DrawPokerGameManager Instance;

    public Deck deck;

    public PlayerHand player;
    public PlayerHand dealer;

    public bool canDraw;

    void Start()
    {
        StartNewRound();
    }

    void Awake()
    {
        Instance = this;
    }

    public void StartNewRound()
    {
        deck.Initialize();

        player = new PlayerHand();
        dealer = new PlayerHand();

        player.Draw(deck, 5);
        dealer.Draw(deck, 5);

        canDraw = true;

        UIManager.Instance.RefreshHands();
        UIManager.Instance.ShowResult("");
    }

    public void PlayerDraw(List<int> discard)
    {
        if (!canDraw) return;

        player.Discard(discard);
        player.Draw(deck, discard.Count);

        DealerAI();

        canDraw = false;

        DetermineWinner();
        UIManager.Instance.RefreshHands();
    }

    void DealerAI()
    {
        Dictionary<Rank, int> counts =
            new Dictionary<Rank, int>();

        foreach (var c in dealer.cards)
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

    void DetermineWinner()
    {
        var p = PokerHandEvaluator.Evaluate(player.cards);
        var d = PokerHandEvaluator.Evaluate(dealer.cards);

        string result =
            p > d ? "Player Wins!" :
            d > p ? "Dealer Wins!" :
            "Tie!";

        UIManager.Instance.ShowResult(
            $"Player: {p}\nDealer: {d}\n{result}");
    }
}

