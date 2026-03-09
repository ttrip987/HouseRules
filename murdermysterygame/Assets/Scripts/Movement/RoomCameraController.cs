using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance;

    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void MoveToRoom(RoomDefinition room)
    {
        if (room == null || cam == null) return;

        transform.position = new Vector3(
            room.RoomCenter.x,
            room.RoomCenter.y,
            transform.position.z
        );

        cam.orthographicSize = room.CameraSize;
    }
}
