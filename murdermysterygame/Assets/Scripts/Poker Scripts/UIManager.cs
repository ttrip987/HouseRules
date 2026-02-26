using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform playerPanel;
    public Transform dealerPanel;
    public GameObject cardPrefab;
    public TMP_Text resultText;

    [Header("Dealer Hidden Card")]
    public Sprite dealerBackSprite;

    [Header("Fan Settings (Player)")]
    public float playerSpacing = 95f;
    public float playerCurve = 18f;
    public float playerRot = 10f;

    [Header("Fan Settings (Dealer)")]
    public float dealerSpacing = 70f;
    public float dealerCurve = 10f;
    public float dealerRot = 6f;

    void Awake()
    {
        Instance = this;
    }

    public void RefreshHands()
    {
        ClearPanel(playerPanel);
        ClearPanel(dealerPanel);

        SpawnHand(DrawPokerGameManager.Instance.player.cards, playerPanel);
        SpawnHand(DrawPokerGameManager.Instance.dealer.cards, dealerPanel);

        ArrangeFan(playerPanel, playerSpacing, playerCurve, playerRot, false);
        ArrangeFan(dealerPanel, dealerSpacing, dealerCurve, dealerRot, true);
    }

    void SpawnHand(List<CardData> handCards, Transform panel)
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, panel);
            cardGO.transform.localScale = Vector3.one;

            RectTransform rt = cardGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localRotation = Quaternion.identity;

            CardView view = cardGO.GetComponent<CardView>();
            if (view != null) view.SetCard(handCards[i], i);
        }
    }

    void ArrangeFan(Transform panel, float spacing, float curve, float rotAmt, bool invert)
    {
        int count = panel.childCount;
        if (count == 0) return;

        float mid = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            float t = i - mid;

            float x = t * spacing;
            float y = (-Mathf.Abs(t) * curve) * (invert ? -1f : 1f);
            float rotZ = (-t * rotAmt) * (invert ? -1f : 1f);

            RectTransform rt = panel.GetChild(i).GetComponent<RectTransform>();
            CardView view = panel.GetChild(i).GetComponent<CardView>();

            if (view != null) view.SetLayout(new Vector2(x, y), rotZ);
            else if (rt != null)
            {
                rt.anchoredPosition = new Vector2(x, y);
                rt.localRotation = Quaternion.Euler(0f, 0f, rotZ);
            }
        }
    }

    void ClearPanel(Transform panel)
    {
        for (int i = panel.childCount - 1; i >= 0; i--)
            Destroy(panel.GetChild(i).gameObject);
    }

    public void ShowResult(string text)
    {
        if (resultText != null) resultText.text = text;
    }

    public void HideDealerCards()
    {
        for (int i = 0; i < dealerPanel.childCount; i++)
        {
            CardView view = dealerPanel.GetChild(i).GetComponent<CardView>();
            if (view != null) view.SetFaceDown(dealerBackSprite);
        }
    }

    public void RevealDealerCards()
    {
        for (int i = 0; i < dealerPanel.childCount; i++)
        {
            CardView view = dealerPanel.GetChild(i).GetComponent<CardView>();
            if (view != null) view.SetFaceUp();
        }
    }

    public void PopUpWinningCards(List<CardData> winningCards, Transform panel)
    {
        for (int i = 0; i < panel.childCount; i++)
        {
            CardView view = panel.GetChild(i).GetComponent<CardView>();
            if (view != null)
                view.SetPop(winningCards != null && winningCards.Contains(view.data));
        }
    }

    public void ResetCardPopups()
    {
        ResetPanel(playerPanel);
        ResetPanel(dealerPanel);
    }

    void ResetPanel(Transform panel)
    {
        for (int i = 0; i < panel.childCount; i++)
        {
            CardView view = panel.GetChild(i).GetComponent<CardView>();
            if (view != null) view.SetPop(false);
        }
    }
}