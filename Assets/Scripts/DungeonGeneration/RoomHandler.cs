using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class RoomHandler : MonoBehaviour {
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

    public RoomConnectorHandler GetRoomSpawnConnector() {
        return roomSpawnConnector;
    }

    public Vector3 GetRoomSpawnVector(RoomConnectorHandler parentRoomConnectorHandler) {
        if (roomSpawnConnector.transform.rotation.eulerAngles.y == 0 || roomSpawnConnector.transform.rotation.eulerAngles.y == 180) {
            return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - (((Quaternion.Euler(0, 180, 0)) * (roomSpawnConnector.transform.rotation)) * (parentRoomConnectorHandler.transform.rotation * roomSpawnConnector.GetDoorwayCollider().transform.position));
        }
        return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - ((roomSpawnConnector.transform.rotation) * (parentRoomConnectorHandler.transform.rotation * roomSpawnConnector.GetDoorwayCollider().transform.position));
    }
}