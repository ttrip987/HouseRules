using UnityEngine;

public class RoomDefinition : MonoBehaviour
{
    public BoxCollider2D roomBounds;
    public float padding = 1f;

    [HideInInspector] public Vector3 RoomCenter;
    [HideInInspector] public float CameraSize;

    private void Awake()
    {
        if (roomBounds == null)
            roomBounds = GetComponent<BoxCollider2D>();

        if (roomBounds == null)
        {
            Debug.LogError("RoomDefinition needs a BoxCollider2D for roomBounds.", this);
            return;
        }

        Bounds b = roomBounds.bounds;
        RoomCenter = new Vector3(b.center.x, b.center.y, 0f);

        Camera cam = Camera.main;
        if (cam == null)
            return;

        float screenAspect = (float)Screen.width / Screen.height;
        float roomWidth = b.size.x + (padding * 2f);
        float roomHeight = b.size.y + (padding * 2f);

        float sizeFromHeight = roomHeight / 2f;
        float sizeFromWidth = roomWidth / (2f * screenAspect);

        CameraSize = Mathf.Max(sizeFromHeight, sizeFromWidth);
    }
}