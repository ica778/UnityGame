using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnectorHandler : MonoBehaviour {
    public enum RoomConnectorType {
        Doorway,
    }

    [SerializeField] private GameObject entranceway;
    [SerializeField] private GameObject wall;
    [SerializeField] private BoxCollider doorwayCollider;
    [SerializeField] private RoomConnectorType roomConnectorType = RoomConnectorType.Doorway;
    private RoomHandler parentRoom;

    private void Awake() {
        parentRoom = GetComponentInParent<RoomHandler>();
    }

    private void Start() {
        
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

    public RoomHandler GetParentRoom() {
        return parentRoom;
    }

    public RoomConnectorType GetRoomConnectorType() {
        return roomConnectorType;
    }

    public BoxCollider GetDoorwayCollider() {
        return doorwayCollider;
    }
}