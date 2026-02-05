using System.Collections.Generic;
using System.Linq;

public static class PokerHandEvaluator
{
    public static PokerHandRank Evaluate(List<Card> hand)
    {
        var ranks = hand.GroupBy(c => c.rank).OrderByDescending(g => g.Count());
        bool flush = hand.All(c => c.suit == hand[0].suit);

        var values = hand.Select(c => (int)c.rank).OrderBy(v => v).ToList();
        bool straight = values.Distinct().Count() == 5 &&
                        values[4] - values[0] == 4;

        if (straight && flush && values.Contains((int)Rank.Ace))
            return PokerHandRank.RoyalFlush;

        if (straight && flush)
            return PokerHandRank.StraightFlush;

        if (ranks.First().Count() == 4)
            return PokerHandRank.FourOfAKind;

        if (ranks.First().Count() == 3 && ranks.ElementAt(1).Count() == 2)
            return PokerHandRank.FullHouse;

        if (flush)
            return PokerHandRank.Flush;

        if (straight)
            return PokerHandRank.Straight;

        if (ranks.First().Count() == 3)
            return PokerHandRank.ThreeOfAKind;

        if (ranks.First().Count() == 2 && ranks.ElementAt(1).Count() == 2)
            return PokerHandRank.TwoPair;

        if (ranks.First().Count() == 2)
            return PokerHandRank.OnePair;

        return PokerHandRank.HighCard;
    }
}
