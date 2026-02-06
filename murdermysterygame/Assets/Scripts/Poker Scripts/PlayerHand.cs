using System.Collections.Generic;

public class PlayerHand
{
    public List<CardData> cards = new List<CardData>();

    public void Draw(Deck deck, int amount)
    {
        for (int i = 0; i < amount; i++)
            cards.Add(deck.Draw());
    }

    public void Discard(List<int> indices)
    {
        indices.Sort((a, b) => b.CompareTo(a));

        foreach (int i in indices)
            cards.RemoveAt(i);
    }
}

