using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject peopleMenu;
    public GameObject howToMenu;

    AudioSource FlipPage;

    bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
        if (settingsMenu) settingsMenu.SetActive(false);
        if (peopleMenu) peopleMenu.SetActive(false);
        if (howToMenu) howToMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        
        if (isPaused && (IsOpen(settingsMenu) || IsOpen(peopleMenu) || IsOpen(howToMenu)))
        {
            OpenPauseMenu();
            return;
        }

     
        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        OpenPauseMenu();
        FlipPage.Play();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        SetAllMenus(false);
        FlipPage.Play();

    }

    public void OpenPauseMenu()
    {
        SetAllMenus(false);
        if (pauseMenu) pauseMenu.SetActive(true);
        FlipPage.Play();

    }

    public void OpenSettingsMenu()
    {
        SetAllMenus(false);
        if (settingsMenu) settingsMenu.SetActive(true);
        FlipPage.Play();

    }

    public void OpenPeopleMenu()
    {
        SetAllMenus(false);
        if (peopleMenu) peopleMenu.SetActive(true);
        FlipPage.Play();

    }

    public void OpenHowToMenu()
    {
        SetAllMenus(false);
        if (howToMenu) howToMenu.SetActive(true);
        FlipPage.Play();

    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        
    }

    void SetAllMenus(bool active)
    {
        if (pauseMenu) pauseMenu.SetActive(active);
        if (settingsMenu) settingsMenu.SetActive(active);
        if (peopleMenu) peopleMenu.SetActive(active);
        if (howToMenu) howToMenu.SetActive(active);
    }

    bool IsOpen(GameObject go) => go != null && go.activeSelf;
}