public class DealerStrategy
{

    private AIProfile profile;


    public DealerStrategy(AIProfile profile)
    {
        this.profile = profile;
    }



    public DrawDecision Decide(
        PlayerHand hand,
        NPCMemory memory)
    {

        HandAnalysis analysis =
            HandEvaluator.Analyze(hand);


        if(analysis.isStrongHand)
        {
            return DrawDecision.Keep;
        }


        return DrawDecision.Draw;
    }
}