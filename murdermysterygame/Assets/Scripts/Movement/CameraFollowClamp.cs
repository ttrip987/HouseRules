using UnityEngine;

public class CameraFollowClamp : MonoBehaviour
{
    public Transform target;
    public BoxCollider2D bounds;

    public BoxCollider2D currentRoom; 

    float camHalfHeight;
    float camHalfWidth;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || bounds == null)
            return;

        Bounds b = bounds.bounds;

        float minX = b.min.x + camHalfWidth;
        float maxX = b.max.x - camHalfWidth;
        float minY = b.min.y + camHalfHeight;
        float maxY = b.max.y - camHalfHeight;

        float x = Mathf.Clamp(target.position.x, minX, maxX);
        float y = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void SetBounds(BoxCollider2D newBounds)
    {
        
        if (currentRoom == newBounds)
            return;

        currentRoom = newBounds;
        bounds = newBounds;

        transform.position = new Vector3(newBounds.bounds.center.x, newBounds.bounds.center.y, transform.position.z);

        LateUpdate(); 
    }
}