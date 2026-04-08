using UnityEngine;

public class RoomTeleportTrigger : MonoBehaviour
{
    public GameObject player;
    public Transform destination;
    public RoomDefinition targetRoom;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.gameObject != player) return;

        triggered = true;

        if (destination != null)
            player.transform.position = destination.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        if (RoomCameraController.Instance != null && targetRoom != null)
            RoomCameraController.Instance.MoveToRoom(targetRoom);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
            triggered = false;
    }
}