using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Poker/Card")]
public class CardData : ScriptableObject
{
    public Suit suit;
    public Rank rank;
    public Sprite sprite;
}
