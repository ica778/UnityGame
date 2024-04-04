using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    [SerializeField] private LayerMask dungeonRoomOpeningColliderLayer;
    [SerializeField] private GameObject doorway;

    private Queue<RoomConnectorHandler> queue = new Queue<RoomConnectorHandler>();
    private HashSet<RoomConnectorHandler> connectors = new HashSet<RoomConnectorHandler>(); 

    private int maxRooms = 300;
    private int currentRoomCount = 0;


    private void Start () {
        DungeonGeneration();
        ConnectRooms();
    }

    private void DungeonGeneration() {
        GameObject entrance = Instantiate(entranceRoom, transform.position, Quaternion.identity);
        Room entranceRoomScript = entrance.GetComponent<Room>();

        foreach (RoomConnectorHandler i in entranceRoomScript.GetRoomConnectors()) {
            queue.Enqueue(i);
            connectors.Add(i);
        }
        
        while (currentRoomCount < maxRooms && queue.Count > 0) {
            RoomConnectorHandler currentRoomEntrance = queue.Dequeue();
            /*
            int roomID = Random.Range(0, rooms.Length);
            //int roomID = 0;
            Room room = SpawnRoomObject(currentRoomEntrance, roomID);
            */
            ShuffleRooms();
            Room room = null;
            foreach (GameObject currentPrefab in rooms) {
                room = SpawnRoomObject(currentRoomEntrance, currentPrefab);
                if (!ValidateRoomGeneration(room)) {
                    // NOTE: HAVE TO DISABLE OBJECT BECAUSE DESTROY APPEARS TO RUN AFTER START INSTEAD OF DURING
                    room.gameObject.SetActive(false);
                    Destroy(room.gameObject);
                    connectors.Add(currentRoomEntrance);
                    room = null;
                }
                else {
                    break;
                }
            }
            if (!room) {
                continue;
            }
            /*
            if (!ValidateRoomGeneration(room)) {
                // NOTE: HAVE TO DISABLE OBJECT BECAUSE DESTROY APPEARS TO RUN AFTER START INSTEAD OF DURING
                room.gameObject.SetActive(false);
                Destroy(room.gameObject);
                connectors.Add(currentRoomEntrance);
                continue;
            }
            */
            currentRoomCount++;

            foreach (RoomConnectorHandler i in room.GetRoomConnectors()) {
                if (Random.Range(0, 10) < 10) {
                    queue.Enqueue(i);
                }
                connectors.Add(i);
            }
        }
    }

    private void ShuffleRooms() {
        for (int i = rooms.Length - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            GameObject temp = rooms[i];
            rooms[i] = rooms[j];
            rooms[j] = temp;
        }
    }

    private Room SpawnRoomObject(RoomConnectorHandler root, GameObject prefab) {
        Room roomPrefab = prefab.GetComponent<Room>();
        Vector3 spawnRoomPosition = roomPrefab.GetRoomSpawnVector(root);

        GameObject newRoomObject = Instantiate(
            prefab,
            spawnRoomPosition,
            root.transform.rotation
            );
        Room room = newRoomObject.GetComponent<Room>();
        return room;
    }

    private Room SpawnRoomObject(RoomConnectorHandler root, int roomID) {
        Room roomPrefab = rooms[roomID].GetComponent<Room>();
        Vector3 spawnRoomPosition = roomPrefab.GetRoomSpawnVector(root);

        GameObject newRoomObject = Instantiate(
            rooms[roomID],
            spawnRoomPosition,
            root.transform.rotation
            );
        Room room = newRoomObject.GetComponent<Room>();
        return room;
    }

    private bool ValidateRoomGeneration(Room room) {
        if (!room.GetDungeonValidator().CheckIfValid()) {
            return false;
        }
        return true;
    }

    // TODO: could probably optimize this by reducing potential duplicates
    private void ConnectRooms() {
        foreach (RoomConnectorHandler connector in connectors) {
            BoxCollider collider = connector.GetDoorwayCollider();
            Vector3 center = collider.bounds.center;
            Vector3 extents = collider.bounds.extents;
            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, Quaternion.identity, dungeonRoomOpeningColliderLayer);
            if (overlappingColliders.Length == 2) {
                RoomConnectorHandler door1 = overlappingColliders[0].GetComponentInParent<RoomConnectorHandler>();
                RoomConnectorHandler door2 = overlappingColliders[1].GetComponentInParent<RoomConnectorHandler>();
                if (door1) {
                    door1.OpenEntrance();
                }
                if (door2) {
                    door2.OpenEntrance();
                }
                Instantiate(doorway, overlappingColliders[0].transform.position, overlappingColliders[0].transform.rotation);
            }
        }
    }
}