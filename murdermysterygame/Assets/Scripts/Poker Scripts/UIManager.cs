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
    public TextMeshProUGUI resultText;

    List<CardView> playerViews = new();

    void Awake()
    {
        Instance = this;
    }

    public void RefreshHands()
    {
        Clear(playerPanel);
        Clear(dealerPanel);

        playerViews.Clear();

        var game = DrawPokerGameManager.Instance;

        for (int i = 0; i < game.player.cards.Count; i++)
            CreateCard(game.player.cards[i], i, playerPanel, true);

        for (int i = 0; i < game.dealer.cards.Count; i++)
            CreateCard(game.dealer.cards[i], i, dealerPanel, false);
    }

    void CreateCard(CardData card, int index,
                    Transform parent, bool clickable)
    {
        var obj = Instantiate(cardPrefab, parent);
        var view = obj.GetComponent<CardView>();

        view.SetCard(card, index);

        if (clickable)
        {
            obj.GetComponent<Button>()
               .onClick.AddListener(view.ToggleSelect);

            playerViews.Add(view);
        }
    }

    public void OnDrawPressed()
    {
        List<int> discard = new();

        foreach (var v in playerViews)
            if (v.selected)
                discard.Add(v.index);

        DrawPokerGameManager.Instance.PlayerDraw(discard);
    }

    public void ShowResult(string text)
    {
        resultText.text = text;
    }

    void Clear(Transform panel)
    {
        foreach (Transform child in panel)
            Destroy(child.gameObject);
    }
}

