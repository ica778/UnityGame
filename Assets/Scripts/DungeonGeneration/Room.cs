using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private RoomConnectorHandler[] roomEntrances;
    [SerializeField] private RoomConnectorHandler roomSpawnConnector;
    [SerializeField] private DungeonValidator dungeonValidator;

    private void Start() {
    }

    public RoomConnectorHandler[] GetRoomConnectors() {
        return roomEntrances;
    }

    public RoomConnectorHandler GetRoomConnector(int index) {
        return roomEntrances[index];
    }

    public DungeonValidator GetDungeonValidator() {
        return dungeonValidator;
    }

    public void CoupleRoomToCouplingPoint(Transform otherRoomCouplingPoint) {
        transform.position += otherRoomCouplingPoint.transform.position - roomSpawnConnector.GetDoorwayCollider().transform.position;
    }
}