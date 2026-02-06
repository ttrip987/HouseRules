using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> allCards;
    private List<CardData> deck = new List<CardData>();

    public void Initialize()
    {
        deck = new List<CardData>(allCards);
        Shuffle();
    }

    void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    public CardData Draw()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }
}

