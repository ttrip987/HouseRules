using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public DialogueNode startingNode;

    private bool playerInRange;
    private bool hasSpoken;
    private Dialogue dialogue;

    void Awake()
    {
        dialogue = FindObjectOfType<Dialogue>();
    }

    void Update()
    {
        if (playerInRange && !hasSpoken && Input.GetKeyDown(KeyCode.Space))
        {
            dialogue.StartDialogue(startingNode);
            hasSpoken = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hasSpoken = false; // remove if NPC should talk only once
        }
    }
}