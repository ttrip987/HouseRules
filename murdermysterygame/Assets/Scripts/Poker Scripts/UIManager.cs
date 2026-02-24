using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform playerPanel;
    public Transform dealerPanel;
    public GameObject cardPrefab;
    public TMP_Text resultText;

    public float cardSpacing = 40f; 

    void Awake() { Instance = this; }

    public void RefreshHands()
    {
        ClearPanel(playerPanel);
        ClearPanel(dealerPanel);

        SpawnHand(DrawPokerGameManager.Instance.player.cards, playerPanel);
        SpawnHand(DrawPokerGameManager.Instance.dealer.cards, dealerPanel);
    }

    void SpawnHand(List<CardData> handCards, Transform panel)
    {
        int count = handCards.Count;
        float startX = -cardSpacing * (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            CardData cardData = handCards[i];
            GameObject cardGO = Instantiate(cardPrefab, panel);
            CardView cardView = cardGO.GetComponent<CardView>();
            if (cardView != null) cardView.SetCard(cardData, i);

            RectTransform rt = cardGO.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * cardSpacing, 0);
        }
    }

    void ClearPanel(Transform panel)
    {
        for (int i = panel.childCount - 1; i >= 0; i--)
        {
            GameObject childGO = panel.GetChild(i).gameObject;
            Destroy(childGO);
        }
    }

    public void ShowResult(string text)
    {
        if (resultText != null)
            resultText.text = text;
    }

    public void PopUpWinningCards(List<CardData> winningCards, Transform panel)
    {
        for (int i = 0; i < panel.childCount; i++)
        {
            CardView cardView = panel.GetChild(i).GetComponent<CardView>();
            if (cardView != null)
                cardView.PopUp(winningCards.Contains(cardView.data));
        }
    }

    public void ResetCardPopups()
    {
        for (int i = 0; i < playerPanel.childCount; i++)
        {
            CardView cardView = playerPanel.GetChild(i).GetComponent<CardView>();
            if (cardView != null) cardView.PopUp(false);
        }

        for (int i = 0; i < dealerPanel.childCount; i++)
        {
            CardView cardView = dealerPanel.GetChild(i).GetComponent<CardView>();
            if (cardView != null) cardView.PopUp(false);
        }
    }

    public void PlayerDiscard()
    {
        List<int> discardIndices = new List<int>();

        for (int i = 0; i < UIManager.Instance.playerPanel.childCount; i++)
        {
            CardView card = UIManager.Instance.playerPanel.GetChild(i).GetComponent<CardView>();
            if (card != null && card.selected)
                discardIndices.Add(i);
        }

        if (discardIndices.Count > 0)
        {
            DrawPokerGameManager.Instance.player.Discard(discardIndices);
            DrawPokerGameManager.Instance.player.Draw(DrawPokerGameManager.Instance.deck, discardIndices.Count);

            DrawPokerGameManager.Instance.DealerAI(); 
            DrawPokerGameManager.Instance.DetermineWinner();

            UIManager.Instance.RefreshHands();
            UIManager.Instance.ResetCardPopups();
        }
    }
}