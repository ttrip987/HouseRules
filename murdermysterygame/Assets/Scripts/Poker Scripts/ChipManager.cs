using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance;

    [Header("Chips and Pot")]
    public int playerChips = 1000;
    public int dealerChips = 1000;
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

    void Awake()
    {
        Instance = this;
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


    public bool IsGameOver()
    {
        if (playerChips <= 0 || dealerChips <= 0)
        {
            gameOverText.text = playerChips <= 0 ? "DEALER WINS THE MATCH!" : "PLAYER WINS THE MATCH!";
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        if (playerText) playerText.text = $"Player: {playerChips}";
        if (dealerText) dealerText.text = $"Dealer: {dealerChips}";
        if (potText) potText.text = $"Pot: {pot}";

        // Update chip stacks visually
        UpdateChipStack(playerChips, playerChipParent);
        UpdateChipStack(dealerChips, dealerChipParent);
    }


    void UpdateChipStack(int amount, GameObject parent)
    {

        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);


        List<(int, Sprite)> chipValues = new List<(int, Sprite)>()
        {
            (10000, chip500Sprite),
            (1000, chip100Sprite),
            (500, chip25Sprite),
            (100, chip5Sprite),
            (50, chip1Sprite)
        };

        float yOffset = 0f; 
        foreach (var (value, sprite) in chipValues)
        {
            int count = amount / value;
            amount %= value;

            for (int i = 0; i < count; i++)
            {
                GameObject chipGO = Instantiate(chipPrefab, parent.transform);
                chipGO.GetComponent<Image>().sprite = sprite;

                RectTransform rt = chipGO.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, yOffset);
                yOffset += 10f; 
            }
        }
    }

    public void SplitPot()
    {
        int half = pot / 2;
        playerChips += half;
        dealerChips += pot - half;
        pot = 0;
        UpdateUI();
    }
}