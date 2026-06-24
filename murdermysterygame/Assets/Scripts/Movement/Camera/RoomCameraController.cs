using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance;

    public bool smoothTransition = true;
    public float moveSpeed = 8f;
    public float zoomSpeed = 8f;

    private Camera cam;
    private Vector3 targetPosition;
    private float targetSize;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();

        targetPosition = transform.position;
        targetSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (smoothTransition)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            transform.position = targetPosition;
            cam.orthographicSize = targetSize;
        }
    }

    public void MoveToRoom(RoomDefinition room)
    {
        if (room == null || cam == null)
            return;

        Vector3 roomCenter = room.GetRoomCenter();
        float roomSize = room.GetCameraSize(cam);

        targetPosition = new Vector3(
            roomCenter.x,
            roomCenter.y,
            transform.position.z
        );

        targetSize = roomSize;

        if (!smoothTransition)
        {
            transform.position = targetPosition;
            cam.orthographicSize = targetSize;
        }
    }

    public void SnapToRoom(RoomDefinition room)
    {
        if (room == null || cam == null)
            return;

        Vector3 roomCenter = room.GetRoomCenter();
        float roomSize = room.GetCameraSize(cam);

        targetPosition = new Vector3(
            roomCenter.x,
            roomCenter.y,
            transform.position.z
        );

        targetSize = roomSize;

        transform.position = targetPosition;
        cam.orthographicSize = targetSize;
    }
}