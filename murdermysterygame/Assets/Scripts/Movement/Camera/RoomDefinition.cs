using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public BoxCollider2D roomBounds;
    public float padding = 1f;

    public Vector3 GetRoomCenter()
    {
        if (roomBounds == null)
            roomBounds = GetComponent<BoxCollider2D>();

        if (roomBounds == null)
        {
            Debug.LogError("RoomDefinition needs a BoxCollider2D.", this);
            return transform.position;
        }

        Bounds b = roomBounds.bounds;
        return new Vector3(b.center.x, b.center.y, 0f);
    }

    public float GetCameraSize(Camera cam)
    {
        if (roomBounds == null)
            roomBounds = GetComponent<BoxCollider2D>();

        if (roomBounds == null || cam == null)
        {
            Debug.LogError("Missing roomBounds or camera.", this);
            return 5f;
        }

        Bounds b = roomBounds.bounds;

        float screenAspect = cam.aspect;
        float roomWidth = b.size.x + (padding * 2f);
        float roomHeight = b.size.y + (padding * 2f);

        float sizeFromHeight = roomHeight / 2f;
        float sizeFromWidth = roomWidth / (2f * screenAspect);

        return Mathf.Max(sizeFromHeight, sizeFromWidth);
    }
}