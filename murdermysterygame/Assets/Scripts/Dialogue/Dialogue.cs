using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueNode nextNode;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(2, 5)]
    public string line;

    public string speakerName; 

    public DialogueChoice[] choices;
}

public class Dialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogueUI;
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.04f;
    public TextMeshProUGUI nameText;

    [Header("Choices")]
    public GameObject choiceButtonPrefab;
    public Transform choiceContainer;

    private DialogueNode currentNode;
    private bool isTyping;

    private PlayerController playerController;

    

    void Awake()
    {
        dialogueUI.SetActive(false);
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!dialogueUI.activeSelf)
            return;

        // Only allow Space to continue if there are NO choices
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            && !isTyping
            && (currentNode.choices == null || currentNode.choices.Length == 0))
        {
            EndDialogue();
        }
    }

    // ENTRY POINT (called by NPC)
    public void StartDialogue(DialogueNode startingNode)
    {
        dialogueUI.SetActive(true);
        LockPlayer(true);
        ShowNode(startingNode);
    }


    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textComponent.text = "";

        foreach (char c in line)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void CreateChoices(DialogueChoice[] choices)
    {
        foreach (DialogueChoice choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (choice.nextNode != null)
                    ShowNode(choice.nextNode);
                else
                    EndDialogue();
            });
        }
    }

    void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        ClearChoices();

        dialogueUI.SetActive(false);
        LockPlayer(false);
    }

    void LockPlayer(bool lockState)
    {
        if (playerController == null)
            return;

        playerController.canMove = !lockState;

        Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    void ShowNode(DialogueNode node)
    {
        StopAllCoroutines();
        ClearChoices();

        currentNode = node;

        if (nameText != null)
            nameText.text = node.speakerName;  

        StartCoroutine(TypeLine(node.line));

        if (node.choices != null && node.choices.Length > 0)
            CreateChoices(node.choices);
    }
}