using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public CardData data;
    public int index;
    public bool selected;

    public float popAmount = 35f;
    public float popSpeed = 12f;

    RectTransform rt;
    Image img;

    Vector2 basePos;
    float baseRotZ;

    float currentPop;
    float targetPop;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    void Update()
    {
        currentPop = Mathf.Lerp(currentPop, targetPop, Time.deltaTime * popSpeed);
        rt.anchoredPosition = basePos + new Vector2(0f, currentPop);
        rt.localRotation = Quaternion.Euler(0f, 0f, baseRotZ);
    }

    public void SetCard(CardData card, int i)
    {
        data = card;
        index = i;

        if (img != null) img.sprite = card.sprite;

        selected = false;
        targetPop = 0f;
        currentPop = 0f;
    }

    public void SetLayout(Vector2 pos, float rotZ)
    {
        basePos = pos;
        baseRotZ = rotZ;
    }

    public void ToggleSelect()
    {
        selected = !selected;
        targetPop = selected ? popAmount : 0f;
    }

    public void SetPop(bool up)
    {
        selected = up;
        targetPop = up ? popAmount : 0f;
    }
}