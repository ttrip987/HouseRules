using System.Collections.Generic;
using System.Linq;

public static class HandEvaluator
{
    public enum HandRank
    {
        HighCard = 1,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public static (long score, List<CardData> winningCards, string handName) EvaluateHandWithCards(List<CardData> cards)
    {
        cards = cards.OrderByDescending(c => c.rank).ToList();
        List<CardData> winningCards = new List<CardData>();
        string handName = "High Card";
        long score = (int)cards[0].rank;

        bool isFlush = cards.All(c => c.suit == cards[0].suit);
        bool isStraight = true;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if ((int)cards[i].rank - 1 != (int)cards[i + 1].rank)
            {
                isStraight = false;
                break;
            }
        }

        var groups = cards.GroupBy(c => c.rank).OrderByDescending(g => g.Count()).ToList();
        int maxCount = groups[0].Count();

        if (isStraight && isFlush)
        {
            handName = cards[0].rank == Rank.Ace ? "Royal Flush" : "Straight Flush";
            winningCards = new List<CardData>(cards);
            score = 900 + (int)cards[0].rank;
        }
        else if (maxCount == 4)
        {
            handName = "Four of a Kind";
            winningCards = new List<CardData>(groups[0]);
            score = 800 + (int)groups[0].Key;
        }
        else if (maxCount == 3 && groups.Count > 1 && groups[1].Count() == 2)
        {
            handName = "Full House";
            winningCards = new List<CardData>(groups[0].Concat(groups[1]));
            score = 700 + (int)groups[0].Key;
        }
        else if (isFlush)
        {
            handName = "Flush";
            winningCards = new List<CardData>(cards);
            score = 600 + (int)cards[0].rank;
        }
        else if (isStraight)
        {
            handName = "Straight";
            winningCards = new List<CardData>(cards);
            score = 500 + (int)cards[0].rank;
        }
        else if (maxCount == 3)
        {
            handName = "Three of a Kind";
            winningCards = new List<CardData>(groups[0]);
            score = 400 + (int)groups[0].Key;
        }
        else if (maxCount == 2 && groups.Count(g => g.Count() == 2) == 2)
        {
            handName = "Two Pair";
            winningCards = new List<CardData>(groups[0].Concat(groups[1]));
            score = 300 + (int)groups[0].Key;
        }
        else if (maxCount == 2)
        {
            handName = "Pair";
            winningCards = new List<CardData>(groups[0]);
            score = 200 + (int)groups[0].Key;
        }
        else
        {
            handName = "High Card";
            winningCards = new List<CardData> { cards[0] };
            score = 100 + (int)cards[0].rank;
        }

        return (score, winningCards, handName);
    }
}