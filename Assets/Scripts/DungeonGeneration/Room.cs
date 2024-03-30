using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private RoomConnectorHandler[] roomEntrances;
    [SerializeField] private RoomConnectorHandler roomSpawnEntrance;
    [SerializeField] private DungeonValidator dungeonValidator;

    private void Start() {
    }

    public RoomConnectorHandler[] GetRoomConnectors() {
        return roomEntrances;
    }

    public RoomConnectorHandler GetRoomConnector(int index) {
        return roomEntrances[index];
    }

    public bool CheckIfRoomPlacementValid() {
        return !dungeonValidator.CheckIfCollidersOverlapWithOtherColliders();
    }

    public DungeonValidator GetDungeonValidator() {
        return dungeonValidator;
    }

    public void MoveRoomInFrontOfOpening(Transform openingTransform) {
        Vector3 parentPosition = transform.position;
        Vector3 childPosition = roomSpawnEntrance.transform.position;
        transform.position += parentPosition - childPosition;
    }
}