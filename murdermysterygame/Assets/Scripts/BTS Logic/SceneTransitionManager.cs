using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Assign the CanvasGroup on your full-screen black Image")]
    public CanvasGroup fadeCanvasGroup;

    public float fadeDuration = 0.75f;

    bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(FadeOutLoadFadeIn(sceneName));
    }

    IEnumerator FadeOutLoadFadeIn(string sceneName)
    {
        isTransitioning = true;

        
        yield return Fade(1f);

        
        yield return SceneManager.LoadSceneAsync(sceneName);

        
        yield return null;

        
        yield return Fade(0f);

        isTransitioning = false;
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("SceneTransitionManager: fadeCanvasGroup is not assigned!");
            yield break;
        }

        fadeCanvasGroup.blocksRaycasts = true;

        float startAlpha = fadeCanvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; 
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.01f;
    }
}