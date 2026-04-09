using UnityEngine;

public class PuzzleInteract : MonoBehaviour
{
    public string flagToSet = "FoundBody";
    public DialogueNodeAsset puzzleFinishNode;

    private bool playerInRange;
    private bool alreadySolved = false;
    private Dialogue dialogue;

    void Awake()
    {
        dialogue = FindObjectOfType<Dialogue>();
        Debug.Log("PuzzleInteract Awake on " + gameObject.name);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed while in range of " + gameObject.name);
            SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        if (alreadySolved)
        {
            Debug.Log("Already solved");
            return;
        }

        alreadySolved = true;

        Debug.Log("SolvePuzzle called on " + gameObject.name);

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetFlag(flagToSet);
            Debug.Log("PuzzleInteract set flag: " + flagToSet);
        }
        else
        {
            Debug.LogError("GameProgress.Instance is NULL");
        }

        if (dialogue != null && puzzleFinishNode != null)
        {
            Debug.Log("Starting puzzle finish dialogue");
            dialogue.StartDialogue(puzzleFinishNode);
        }
        else
        {
            Debug.LogWarning("Dialogue or puzzleFinishNode missing");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered trigger on " + gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player exited trigger on " + gameObject.name);
        }
    }
}