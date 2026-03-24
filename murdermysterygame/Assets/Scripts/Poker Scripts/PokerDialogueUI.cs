using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PokerDialogueUI : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject root;
    public TMP_Text dialogueText;
    public Button continueButton;

    [Header("Choice Buttons")]
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonTexts;

    private Action continueAction;

    void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        if (root != null)
            root.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].gameObject.SetActive(false);
        }

        continueAction = null;
    }

    public void ShowMessage(string message, Action onContinue = null)
    {
        if (root != null)
            root.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = message;

        continueAction = onContinue;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].gameObject.SetActive(false);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    public void OnContinuePressed()
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        Action action = continueAction;
        continueAction = null;

        if (action != null)
            action();
        else
            HideAll();
    }

    public void ShowChoices(string message, string[] choices, Action[] actions)
    {
        if (root != null)
            root.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = message;

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            if (i < choices.Length && i < actions.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);

                if (choiceButtonTexts != null && i < choiceButtonTexts.Length && choiceButtonTexts[i] != null)
                    choiceButtonTexts[i].text = choices[i];

                choiceButtons[i].onClick.RemoveAllListeners();

                Action action = actions[i];
                choiceButtons[i].onClick.AddListener(() =>
                {
                    HideChoiceButtons();
                    action?.Invoke();
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].gameObject.SetActive(false);
        }
    }
}