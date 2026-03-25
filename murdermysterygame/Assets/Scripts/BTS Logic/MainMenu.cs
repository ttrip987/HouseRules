using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Fade")]
    public CanvasGroup mainMenuCanvasGroup;
    public float fadeDuration = 1f;

    [Header("Dialogue After Fade")]
    public Dialogue dialogue;                 
    public DialogueNodeAsset cutsceneStartNode;    

    [Header("Scene Name (after dialogue ends)")]
    public string gameSceneName = "Game";

    private bool isBusy;

    

    void Start()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        if (mainMenuCanvasGroup == null && mainMenuPanel != null)
            mainMenuCanvasGroup = mainMenuPanel.GetComponent<CanvasGroup>();

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
        }

        
        if (dialogue != null)
            dialogue.onDialogueFinished.AddListener(LoadGameScene);
    }

    public void PlayGame()
    {
        if (isBusy) return;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        isBusy = true;

        
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;

            float startA = mainMenuCanvasGroup.alpha;
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                mainMenuCanvasGroup.alpha = Mathf.Lerp(startA, 0f, t / fadeDuration);
                yield return null;
            }

            mainMenuCanvasGroup.alpha = 0f;
        }

        mainMenuPanel.SetActive(false);

        
        if (dialogue != null && cutsceneStartNode != null)
            dialogue.StartDialogue(cutsceneStartNode);

        isBusy = false;
    }

    private void LoadGameScene()
    {
        SceneTransitionManager.Instance.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void LoadGame()
    {
        SaveManager.Instance.LoadGame();
    }
}