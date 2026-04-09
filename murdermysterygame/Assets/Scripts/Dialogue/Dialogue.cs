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

    public float interactCooldown = 0.2f;
    private float nextAllowedInteractTime = 0f;

    public bool CanStartDialogue => Time.time >= nextAllowedInteractTime;

    private bool lockPlayerOnOpen = true;

    void Awake()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (dialogueUI == null || !dialogueUI.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = currentNode.line;
                isTyping = false;
                return;
            }

            if (currentNode.choices != null && currentNode.choices.Length > 0)
                return;

            if (currentNode.nextNode != null)
                ShowNode(currentNode.nextNode);
            else
                EndDialogue();
        }
    }

    public void StartDialogue(DialogueNodeAsset startingNode)
    {
        StartDialogue(startingNode, true);
    }

    public void StartDialogue(DialogueNodeAsset startingNode, bool lockPlayer)
    {
        if (!CanStartDialogue) return;
        if (IsOpen) return;
        if (startingNode == null) return;

        lockPlayerOnOpen = lockPlayer;

        dialogueUI.SetActive(true);

        if (lockPlayerOnOpen)
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


    void CreateChoices(DialogueChoiceAsset[] choices)
    {
        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                HandleChoice(choice);
            });
        }
    }

    void HandleChoice(DialogueChoiceAsset choice)
    {
        // Check requirement first
        if (!string.IsNullOrEmpty(choice.requiredFlag))
        {
            if (GameProgress.Instance == null || !GameProgress.Instance.HasFlag(choice.requiredFlag))
            {
                Debug.Log("Requirement not met: " + choice.requiredFlag);

                if (choice.failNode != null)
                {
                    ShowNode(choice.failNode);
                }

                return;
            }
        }

        // Load poker scene if this choice is meant to do that
        if (choice.action == DialogueAction.LoadPokerScene)
        {
            EndDialogue();
            Debug.Log("LOADING SCENE: " + pokerSceneName);
            SceneTransitionManager.Instance.LoadScene(pokerSceneName);
            SceneTransitionManager.Instance.LoadScene(pokerSceneName);
            return;
        }

        // Continue dialogue normally
        if (choice.nextNode != null)
            ShowNode(choice.nextNode);
        else
            EndDialogue();
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

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (lockPlayerOnOpen)
            LockPlayer(false);

        lockPlayerOnOpen = true;

        nextAllowedInteractTime = Time.time + interactCooldown;
        onDialogueFinished?.Invoke();
    }

    void LockPlayer(bool lockState)
    {
        if (playerController == null) return;

        playerController.canMove = !lockState;

        Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public void ForceCloseDialogue()
    {
        StopAllCoroutines();
        ClearChoices();

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (lockPlayerOnOpen)
            LockPlayer(false);

        lockPlayerOnOpen = true;
        isTyping = false;
        currentNode = null;
    }

    void ShowNode(DialogueNodeAsset node)
    {
        StopAllCoroutines();
        ClearChoices();

        currentNode = node;

    
        if (!string.IsNullOrEmpty(node.flagToSetOnEnter))
        {
            GameProgress.Instance.SetFlag(node.flagToSetOnEnter);
        }

        if (nameText != null)
            nameText.text = node.speakerName;

        StartCoroutine(TypeLine(node.line));

        if (node.choices != null && node.choices.Length > 0)
            CreateChoices(node.choices);
    }
}