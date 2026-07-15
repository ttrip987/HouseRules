using UnityEngine;

public class DealerAI : MonoBehaviour
{
    public AIProfile profile;

    private DealerStrategy strategy;
    private NPCMemory memory;


    public void Initialize()
    {
        strategy = new DealerStrategy(profile);
        memory = new NPCMemory();
    }


    public DrawDecision MakeDecision(PlayerHand hand)
    {
        return strategy.Decide(hand, memory);
    }
}