using UnityEngine;
using UnityEngine.SceneManagement;

public enum GamePhase
{
    Investigation,
    Poker,
    EndDay
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentDay = 1;
    public GamePhase currentPhase;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDay(int day)
    {
        currentDay = day;
        currentPhase = GamePhase.Investigation;
        SceneManager.LoadScene("InvestigationScene");
    }

    public void StartPoker()
    {
        currentPhase = GamePhase.Poker;
        SceneManager.LoadScene("PokerScene");
    }

    public void EndDay()
    {
        currentPhase = GamePhase.EndDay;
        currentDay++;
        SaveSystem.SaveGame();
        StartDay(currentDay);
    }

    public void OnPokerWin()
    {
        GameManager.Instance.EndDay();
    }

    public void OnPokerLose()
    {
    // Not coded in yet will once we decide what we want to do. 
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null) return;

        currentDay = data.currentDay;
        currentPhase = GamePhase.Investigation;

        SceneManager.LoadScene("InvestigationScene");
    }
}
