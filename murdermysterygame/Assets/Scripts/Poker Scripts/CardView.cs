using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardView : MonoBehaviour
{
    public Image image;

    public CardData data;
    public int index;
    public bool selected;

    private RectTransform rectTransform;
    private Vector2 basePosition;

    public float liftAmount = 40f;
    public float moveSpeed = 10f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetCard(CardData card, int i)
    {
        data = card;
        index = i;
        image.sprite = card.sprite;

        basePosition = rectTransform.anchoredPosition;
        selected = false;
    }

    public void ToggleSelect()
    {
        selected = !selected;
        StopAllCoroutines();
        StartCoroutine(MoveCard(selected));
    }

    IEnumerator MoveCard(bool up)
    {
        Vector2 target = up
            ? basePosition + Vector2.up * liftAmount
            : basePosition;

        while (Vector2.Distance(rectTransform.anchoredPosition, target) > 0.1f)
        {
            rectTransform.anchoredPosition =
                Vector2.Lerp(rectTransform.anchoredPosition, target, Time.deltaTime * moveSpeed);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }
}
