using UnityEngine;

public class RoomStartSetter : MonoBehaviour
{
    public RoomDefinition startingRoom;

    void Start()
    {
        if (RoomCameraController.Instance != null && startingRoom != null)
            RoomCameraController.Instance.SnapToRoom(startingRoom);
    }
}
