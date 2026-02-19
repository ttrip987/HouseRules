using UnityEngine;
using UnityEngine.UI;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance;

    public int playerChips = 1000;  // starting chips
    public int dealerChips = 1000;
    public int pot = 0;

    public Text playerChipsText;
    public Text dealerChipsText;
    public Text potText;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddToPot(int amount)
    {
        pot += amount;
        UpdateUI();
    }

    public bool PlayerSpend(int amount)
    {
        if (playerChips >= amount)
        {
            playerChips -= amount;
            pot += amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public bool DealerSpend(int amount)
    {
        if (dealerChips >= amount)
        {
            dealerChips -= amount;
            pot += amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void PayoutPlayer()
    {
        playerChips += pot;
        pot = 0;
        UpdateUI();
    }

    public void PayoutDealer()
    {
        dealerChips += pot;
        pot = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerChipsText != null)
            playerChipsText.text = $"Player Chips: {playerChips}";
        if (dealerChipsText != null)
            dealerChipsText.text = $"Dealer Chips: {dealerChips}";
        if (potText != null)
            potText.text = $"Pot: {pot}";
    }
}
