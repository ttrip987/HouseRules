public enum Suit { Hearts, Diamonds, Clubs, Spades }
public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven,
    Eight, Nine, Ten, Jack, Queen, King, Ace
}

[System.Serializable]
public class Card
{
    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public override string ToString()
    {
        return $"{rank} of {suit}";
    }
}