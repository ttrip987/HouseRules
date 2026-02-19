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

    void Awake()
    {
        Instance = this;
    }

    public void RefreshHands()
    {
        ClearPanel(playerPanel);
        ClearPanel(dealerPanel);

        DrawHand(DrawPokerGameManager.Instance.player.cards, playerPanel, false);
        DrawHand(DrawPokerGameManager.Instance.dealer.cards, dealerPanel, true);
    }

    void DrawHand(List<CardData> cards, Transform panel, bool hideCards)
    {
        float spacing = 20f;
        float startX = -spacing * (cards.Count - 1) / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, panel);

            RectTransform rt = cardGO.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacing, 0);

            float angle = 0f;

            if (cards.Count > 1)
                angle = Mathf.Lerp(15f, -15f, i / (float)(cards.Count - 1));

            rt.rotation = Quaternion.Euler(0, 0, angle);

            CardView view = cardGO.GetComponent<CardView>();

            if (hideCards && DrawPokerGameManager.Instance.canDraw)
            {
                view.image.sprite = null;
            }
            else
            {
                view.SetCard(cards[i], i);
            }
        }
    }

    void ClearPanel(Transform panel)
    {
        foreach (Transform child in panel)
            Destroy(child.gameObject);
    }

    public void ShowResult(string text)
    {
        if (resultText != null)
            resultText.text = text;
    }
}
