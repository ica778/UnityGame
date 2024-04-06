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

    public Vector3 GetRoomSpawnVector(RoomConnectorHandler origin) {
        return origin.GetDoorwayCollider().transform.position - (origin.transform.rotation * roomSpawnConnector.GetDoorwayCollider().transform.position);
    }
}