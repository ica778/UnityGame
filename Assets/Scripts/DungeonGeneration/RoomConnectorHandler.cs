using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnectorHandler : MonoBehaviour {
    public enum RoomConnectorType {
        Doorway,
        SpawnEntranceDoorway,
    }

    [SerializeField] private GameObject entranceway;
    [SerializeField] private GameObject wall;
    [SerializeField] private RoomConnectorType roomConnectorType = RoomConnectorType.Doorway;
    private Room parentRoom;


    private void Awake() {
        parentRoom = GetComponentInParent<Room>();
    }

    public void OpenEntrance() {
        entranceway.SetActive(true);
        wall.SetActive(false);
    }

    public void CloseEntrance() {
        entranceway.SetActive(false);
        wall.SetActive(true);
    }

    public GameObject GetEntranceway() {
        return entranceway;
    }

    public GameObject GetWall() { 
        return wall; 
    }

    public Room GetParentRoom() {
        return parentRoom;
    }

    public RoomConnectorType GetRoomConnectorType() {
        return roomConnectorType;
    }
}