using UnityEngine;

public class RoomHandler : MonoBehaviour {
    [SerializeField] private RoomConnectorHandler[] roomEntrances;
    [SerializeField] private RoomConnectorHandler[] roomSpawnConnectors;
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

    public RoomConnectorHandler[] GetRoomSpawnConnectors() {
        return roomSpawnConnectors;
    }

    public void DisableConnectorColliders() {
        foreach (RoomConnectorHandler roomEntranceConnectorHandler in roomEntrances) {
            roomEntranceConnectorHandler.GetDoorwayCollider().gameObject.SetActive(false);
        }
    }

    public void EnableConnectorColliders() {
        foreach (RoomConnectorHandler roomEntranceConnectorHandler in roomEntrances) {
            roomEntranceConnectorHandler.GetDoorwayCollider().gameObject.SetActive(true);
        }
    }

    public Vector3 GetRoomSpawnVector(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectHandler) {
        bool containsRoomConnectorHandler = false;
        foreach (RoomConnectorHandler x in roomSpawnConnectors) {
            if (newRoomConnectHandler.Equals(x)) {
                containsRoomConnectorHandler = true;
                break;
            }
        }

        if (!containsRoomConnectorHandler) {
            Debug.LogException(new System.Exception("ROOM DOES NOT CONTAIN THIS OPENING"), this);
        }
        /*
        if (newRoomConnectHandler.transform.rotation.eulerAngles.y == 0 || newRoomConnectHandler.transform.rotation.eulerAngles.y == 180) {
            return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - (((Quaternion.Euler(0, 180, 0)) * (newRoomConnectHandler.transform.rotation)) * (parentRoomConnectorHandler.transform.rotation * newRoomConnectHandler.GetDoorwayCollider().transform.position));
        }
        return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - ((newRoomConnectHandler.transform.rotation) * (parentRoomConnectorHandler.transform.rotation * newRoomConnectHandler.GetDoorwayCollider().transform.position));
        */
        return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - (GetSpawnNewRoomObjectQuaternion(parentRoomConnectorHandler, newRoomConnectHandler) * (newRoomConnectHandler.GetDoorwayCollider().transform.position));

    }

    // TODO: fix this duplicate code for getting quaternion rotation
    private Quaternion GetSpawnNewRoomObjectQuaternion(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler) {
        Quaternion rotation = newRoomConnectorHandler.transform.rotation * Quaternion.Euler(0, 180, 0);

        return rotation * parentRoomConnectorHandler.transform.rotation;
    }
}