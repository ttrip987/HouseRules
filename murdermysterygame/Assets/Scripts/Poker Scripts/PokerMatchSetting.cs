using UnityEngine;

public class PokerMatchSettings : MonoBehaviour
{
    [Header("Match Info")]
    public string dealerName = "Wesley";

    [Header("Starting Chips")]
    public int startingPlayerChips = 1000;
    public int startingDealerChips = 1000;

    [Header("Scene Results")]
    public string playerLoseScene;
    public string dealerLoseScene;
}
