using System.Collections.Generic;
using System.Linq;

public static class HandEvaluator
{

    public static long EvaluateHand(List<CardData> hand)
    {

        var ranks = hand.Select(c => (int)c.rank).OrderByDescending(r => r).ToList();
        var suits = hand.Select(c => c.suit).ToList();

        bool isFlush = suits.Distinct().Count() == 1;

        bool isStraight = true;
        for (int i = 0; i < ranks.Count - 1; i++)
        {
            if (ranks[i] - 1 != ranks[i + 1])
            {
                isStraight = false;
                break;
            }
        }


        var groups = hand.GroupBy(c => c.rank)
                         .OrderByDescending(g => g.Count()) 
                         .ThenByDescending(g => (int)g.Key) 
                         .ToList();

        int mainCount = groups[0].Count();
        int mainRank = (int)groups[0].Key;

        int secondCount = groups.Count > 1 ? groups[1].Count() : 0;
        int secondRank = groups.Count > 1 ? (int)groups[1].Key : 0;


        long score = 0;


        if (isStraight && isFlush && ranks[0] == 14) score = 10_000_000;     
        else if (isStraight && isFlush) score = 9_000_000;                  
        else if (mainCount == 4) score = 8_000_000 + mainRank * 100 + ranks.Where(r => r != mainRank).First(); 
        else if (mainCount == 3 && secondCount == 2) score = 7_000_000 + mainRank * 100 + secondRank;            
        else if (isStraight) score = 5_000_000 + ranks[0];                    
        else if (mainCount == 3) score = 4_000_000 + mainRank * 10000 + RanksToNumber(ranks.Where(r => r != mainRank).ToList()); 
        else if (mainCount == 2 && secondCount == 2) score = 3_000_000 + mainRank * 10000 + secondRank * 100 + ranks.Where(r => r != mainRank && r != secondRank).First(); 
        else if (mainCount == 2) score = 2_000_000 + mainRank * 10000 + RanksToNumber(ranks.Where(r => r != mainRank).ToList()); 
        else score = 1_000_000 + RanksToNumber(ranks);                      

        return score;
    }


    private static long RanksToNumber(List<int> ranks)
    {
        long result = 0;
        foreach (var r in ranks)
        {
            result = result * 100 + r; 
        }
        return result;
    }
}
