using UnityEngine;

public class RoomConnectorHandler : MonoBehaviour {
    public enum RoomConnectorType {
        Doorway,
    }

    [SerializeField] private GameObject entranceway;
    [SerializeField] private GameObject wall;
    [SerializeField] private BoxCollider doorwayCollider;
    [SerializeField] private RoomConnectorType roomConnectorType = RoomConnectorType.Doorway;

    public void OpenEntrance() {
        entranceway.SetActive(true);
        wall.SetActive(false);
        doorwayCollider.gameObject.SetActive(false);
    }

    public void CloseEntrance() {
        entranceway.SetActive(false);
        wall.SetActive(true);
        doorwayCollider.gameObject.SetActive(true);
    }

    public GameObject GetEntranceway() {
        return entranceway;
    }

    public GameObject GetWall() { 
        return wall; 
    }

    public RoomConnectorType GetRoomConnectorType() {
        return roomConnectorType;
    }

    public BoxCollider GetDoorwayCollider() {
        return doorwayCollider;
    }
}