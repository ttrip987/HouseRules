using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> allCards; // Assign 52 cards in Inspector

    private List<CardData> currentDeck;

    void Awake()
    {
        ResetDeck();
    }

    public void ResetDeck()
    {
        currentDeck = new List<CardData>(allCards);
        Shuffle();
    }

    void Shuffle()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            CardData temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
    }

    public CardData DrawCard()
    {
        if (currentDeck.Count == 0)
        {
            Debug.LogError("Deck empty!");
            return null;
        }

        CardData card = currentDeck[0];
        currentDeck.RemoveAt(0);
        return card;
    }
}

