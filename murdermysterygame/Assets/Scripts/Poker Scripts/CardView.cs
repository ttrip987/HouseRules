using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public CardData data;
    public int index;
    public bool selected;

    private RectTransform rt;
    private Image img;

    [Header("Pop Settings")]
    public float popAmount = 35f;
    public float popSpeed = 12f;

    private Vector2 basePos;
    private float baseRotZ;

    private float currentPop;
    private float targetPop;

    private bool isFaceDown = false;
    private Sprite faceSprite = null;

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

        faceSprite = (card != null) ? card.sprite : null;
        isFaceDown = false;

        if (img != null && faceSprite != null)
            img.sprite = faceSprite;

        selected = false;
        currentPop = 0f;
        targetPop = 0f;
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

    public void SetFaceDown(Sprite backSprite)
    {
        isFaceDown = true;

        if (img != null && backSprite != null)
            img.sprite = backSprite;
    }

    public void SetFaceUp()
    {
        isFaceDown = false;

        if (img != null && faceSprite != null)
            img.sprite = faceSprite;
    }
}