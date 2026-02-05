using UnityEngine;

public class RoomTeleportTrigger : MonoBehaviour
{
    public GameObject player;              
    public Transform destination;          
    public BoxCollider2D newCameraBounds; 

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (triggered) return;
        if (other.gameObject != player) return;

        
        player.transform.position = destination.position;

        
        CameraFollowClamp cam = Camera.main.GetComponent<CameraFollowClamp>();
        cam.SetBounds(newCameraBounds);

       
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        triggered = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.gameObject == player)
            triggered = false;
    }
}