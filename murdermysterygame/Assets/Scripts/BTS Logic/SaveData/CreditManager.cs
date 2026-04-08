using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance;

    [Header("Credits")]
    public int credits = 0;

    [Header("UI")]
    public TMP_Text creditsText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RefreshUI();
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        if (credits < 0) credits = 0;
        RefreshUI();
    }

    public bool SpendCredits(int amount)
    {
        if (credits < amount)
            return false;

        credits -= amount;
        RefreshUI();
        return true;
    }

    public void SetCredits(int amount)
    {
        credits = Mathf.Max(0, amount);
        RefreshUI();
    }

    public int GetCredits()
    {
        return credits;
    }

    public void RefreshUI()
    {
        if (creditsText != null)
            creditsText.text = "Credits: " + credits;
    }
}