using UnityEngine;

public class RoomBlocker : MonoBehaviour
{
    [Header("Access")]
    public bool canEnter = false;

    [Header("Block Settings")]
    public Transform pushBackPoint;

    [Header("Dialogue")]
    public DialogueNodeAsset blockedNode;   

    private Dialogue dialogue;

    void Start()
    {
        dialogue = FindObjectOfType<Dialogue>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (canEnter)
            return;

        
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        
        if (pushBackPoint != null)
            other.transform.position = pushBackPoint.position;

       
        if (dialogue != null && blockedNode != null && !dialogue.IsOpen)
        {
            dialogue.StartDialogue(blockedNode);
        }
    }

    public void UnlockRoom()
    {
        canEnter = true;
    }

    public void LockRoom()
    {
        canEnter = false;
    }
}