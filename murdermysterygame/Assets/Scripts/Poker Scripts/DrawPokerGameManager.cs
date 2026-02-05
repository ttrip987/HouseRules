using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawPokerGameManager : MonoBehaviour
{
    Deck deck;
    PlayerHand player;
    PlayerHand dealer;

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        deck = new Deck();
        player = new PlayerHand();
        dealer = new PlayerHand();

        player.Draw(deck, 5);
        dealer.Draw(deck, 5);

        Debug.Log("Player Hand:");
        PrintHand(player.cards);

        // Example: player discards card 1 & 3
        PlayerDiscard(new List<int> { 1, 3 });

        DealerAI();

        DetermineWinner();
    }

    public void PlayerDiscard(List<int> discardIndices)
    {
        player.Discard(discardIndices);
        player.Draw(deck, discardIndices.Count);

        Debug.Log("Player After Draw:");
        PrintHand(player.cards);
    }

    void DealerAI()
    {
        // VERY basic AI: discard all non-paired cards
        var groups = dealer.cards.GroupBy(c => c.rank);
        List<int> discard = new List<int>();

        for (int i = 0; i < dealer.cards.Count; i++)
        {
            if (groups.First(g => g.Key == dealer.cards[i].rank).Count() == 1)
                discard.Add(i);
        }

        dealer.Discard(discard);
        dealer.Draw(deck, discard.Count);
    }

    void DetermineWinner()
    {
        var playerRank = PokerHandEvaluator.Evaluate(player.cards);
        var dealerRank = PokerHandEvaluator.Evaluate(dealer.cards);

        Debug.Log($"Player: {playerRank}");
        Debug.Log($"Dealer: {dealerRank}");
    }

    void PrintHand(List<Card> hand)
    {
        foreach (var c in hand)
            Debug.Log(c);
    }
}
