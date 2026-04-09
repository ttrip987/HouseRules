using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance;

    [Header("Chips and Pot")]
    public int playerChips;
    public int dealerChips;
    public int pot = 0;

    [Header("UI Text")]
    public TMP_Text playerText;
    public TMP_Text dealerText;
    public TMP_Text potText;
    public TMP_Text gameOverText;

    [Header("Chip Images")]
    public GameObject playerChipParent;
    public GameObject dealerChipParent;
    public GameObject chipPrefab;
    public Sprite chip1Sprite;
    public Sprite chip5Sprite;
    public Sprite chip25Sprite;
    public Sprite chip100Sprite;
    public Sprite chip500Sprite;

    [Header("Current Match Settings")]
    public PokerMatchSettings currentMatch;

    private string dealerDisplayName = "Dealer";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (currentMatch == null)
            currentMatch = FindObjectOfType<PokerMatchSettings>();

        SetupMatch();
    }

    void SetupMatch()
    {
        if (currentMatch != null)
        {
            playerChips = currentMatch.startingPlayerChips;
            dealerChips = currentMatch.startingDealerChips;
            dealerDisplayName = currentMatch.dealerName;
        }
        else
        {
            playerChips = 1000;
            dealerChips = 1000;
            dealerDisplayName = "Dealer";
        }

        pot = 0;
        UpdateUI();
    }

    public bool PlayerSpend(int amount)
    {
        if (playerChips < amount) return false;
        playerChips -= amount;
        pot += amount;
        UpdateUI();
        return true;
    }

    public bool DealerSpend(int amount)
    {
        if (dealerChips < amount) return false;
        dealerChips -= amount;
        pot += amount;
        UpdateUI();
        return true;
    }

    public int PlayerPayUpTo(int amount)
    {
        int paid = Mathf.Clamp(amount, 0, playerChips);
        playerChips -= paid;
        pot += paid;
        UpdateUI();
        return paid;
    }

    public int DealerPayUpTo(int amount)
    {
        int paid = Mathf.Clamp(amount, 0, dealerChips);
        dealerChips -= paid;
        pot += paid;
        UpdateUI();
        return paid;
    }

    public void PayoutToPlayer()
    {
        playerChips += pot;
        pot = 0;
        UpdateUI();
    }

    public void PayoutToDealer()
    {
        dealerChips += pot;
        pot = 0;
        UpdateUI();
    }

    public void SplitPot()
    {
        int half = pot / 2;
        playerChips += half;
        dealerChips += pot - half;
        pot = 0;
        UpdateUI();
    }

    public bool IsGameOver()
    {
        if (playerChips <= 0)
        {
            LoadResultScene(currentMatch != null ? currentMatch.playerLoseScene : "");
            return true;
        }

        if (dealerChips <= 0)
        {
            LoadResultScene(currentMatch != null ? currentMatch.dealerLoseScene : "");
            return true;
        }

        return false;
    }

    void LoadResultScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("No result scene assigned for this poker match.");
            return;
        }

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    public void UpdateUI()
    {
        if (playerText) playerText.text = $"Player: {playerChips}";
        if (dealerText) dealerText.text = $"{dealerDisplayName}: {dealerChips}";
        if (potText) potText.text = $"Pot: {pot}";

        UpdateChipStack(playerChips, playerChipParent);
        UpdateChipStack(dealerChips, dealerChipParent);
    }

    void UpdateChipStack(int amount, GameObject parent)
    {
        if (parent == null || chipPrefab == null) return;

        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);

        List<(int, Sprite)> chipValues = new List<(int, Sprite)>()
        {
            (500, chip500Sprite),
            (100, chip100Sprite),
            (25, chip25Sprite),
            (5, chip5Sprite),
            (1, chip1Sprite)
        };

        float yOffset = 0f;

        foreach (var chip in chipValues)
        {
            int value = chip.Item1;
            Sprite sprite = chip.Item2;

            if (value <= 0 || sprite == null) continue;

            int count = amount / value;
            amount %= value;

            for (int i = 0; i < count; i++)
            {
                GameObject chipGO = Instantiate(chipPrefab, parent.transform);

                Image img = chipGO.GetComponent<Image>();
                if (img != null)
                    img.sprite = sprite;

                RectTransform rt = chipGO.GetComponent<RectTransform>();
                if (rt != null)
                    rt.anchoredPosition = new Vector2(0, yOffset);

                yOffset += 10f;
            }
        }
    }
}