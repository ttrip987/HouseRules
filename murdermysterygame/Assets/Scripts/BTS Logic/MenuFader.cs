using System;
using System.Collections;
using UnityEngine;

public class MenuFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private float fadeDuration = 1f;

    public void FadeOut(Action onComplete = null)
    {
        if (menuGroup == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(menuGroup.alpha, 0f, onComplete));
    }

    public void FadeIn(Action onComplete = null)
    {
        if (menuGroup == null) return;
        menuGroup.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(menuGroup.alpha, 1f, onComplete));
    }

    private IEnumerator FadeRoutine(float from, float to, Action onComplete)
    {
        // block input while fading
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            menuGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        menuGroup.alpha = to;

        if (to <= 0.001f)
            menuGroup.gameObject.SetActive(false);

        onComplete?.Invoke();
    }

    
}