using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    [Header("Events")]
    public UnityEvent onDialogueFinished;

    [Header("Scene Actions")]
    public string pokerSceneName = "PokerScene"; 

    private DialogueNodeAsset currentNode;
    private bool isTyping;

    private PlayerController playerController;
    public bool IsOpen => dialogueUI != null && dialogueUI.activeSelf;

    public float interactCooldown = 0.2f; // small delay
    private float nextAllowedInteractTime = 0f;

    public bool CanStartDialogue => Time.time >= nextAllowedInteractTime;

    void Awake()
    {
        dialogueUI.SetActive(false);
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!dialogueUI.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = currentNode.line;
                isTyping = false;
                return;
            }

            // If choices exist, wait for click
            if (currentNode.choices != null && currentNode.choices.Length > 0)
                return;

            // Continue chain or end
            if (currentNode.nextNode != null)
                ShowNode(currentNode.nextNode);
            else
                EndDialogue();
        }
    }

    public void StartDialogue(DialogueNodeAsset startingNode)
    {
        if (!CanStartDialogue) return;
        if (IsOpen) return;
        if (startingNode == null) return;

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

    void ShowNode(DialogueNodeAsset node)
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

    void CreateChoices(DialogueChoiceAsset[] choices)
    {
        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
            
                if (choice.action == DialogueAction.LoadPokerScene)
                {
                    EndDialogue(); // close UI + unlock player
                    SceneTransitionManager.Instance.LoadScene(pokerSceneName);
                    return;
                }

            
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

        nextAllowedInteractTime = Time.time + interactCooldown;
        onDialogueFinished?.Invoke();
    }

    void LockPlayer(bool lockState)
    {
        if (playerController == null) return;

        playerController.canMove = !lockState;

        Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }
}