using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Image image;
    public GameObject highlight;
    public CardData data;
    public int index;
    public bool selected;

    public void SetCard(CardData card, int i)
    {
        data = card;
        index = i;
        image.sprite = card.sprite;
        highlight.SetActive(false);
        selected = false;
    }

    public void ToggleSelect()
    {
        selected = !selected;
        highlight.SetActive(selected);
    }
}

