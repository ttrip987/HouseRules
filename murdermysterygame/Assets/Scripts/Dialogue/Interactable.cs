using UnityEngine;

public class Interactable : MonoBehaviour
{
    public DialogueNodeAsset startingNode;

    private bool playerInRange;
    private Dialogue dialogue;

    void Awake()
    {
        dialogue = FindObjectOfType<Dialogue>();
    }

    void Update()
    {
        if (dialogue != null && dialogue.IsOpen)
            return; 

        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (startingNode != null)
                dialogue.StartDialogue(startingNode);

            OnInteract();
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
            playerInRange = false;
    }


    protected virtual void OnInteract()
    {
    }
}

