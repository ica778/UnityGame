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


    private void Awake () {
        DungeonGeneration();
        AddDoors();
    }

    // TODO: make it cycle through all available rooms if one doesn't fit
    private void DungeonGeneration() {
        GameObject entrance = Instantiate(entranceRoom, transform.position, Quaternion.identity);
        Room entranceRoomScript = entrance.GetComponent<Room>();


        foreach (RoomConnectorHandler i in entranceRoomScript.GetRoomConnectors()) {
            queue.Enqueue(i);
            connectors.Add(i);
        }
        Debug.Log("TESTING " + queue.Count);
        while (currentRoomCount < maxRooms && queue.Count > 0) {
            RoomConnectorHandler currentRoomEntrance = queue.Dequeue();
            int roomID = Random.Range(0, rooms.Length);
            //int roomID = 0;
            Room roomPrefab = rooms[roomID].GetComponent<Room>();
            Vector3 spawnRoomPosition = roomPrefab.GetRoomSpawnVector(currentRoomEntrance);
            
            GameObject newRoomObject = Instantiate(
                rooms[roomID],
                //currentRoomEntrance.transform.position,
                spawnRoomPosition,
                currentRoomEntrance.transform.rotation
                );
            Room newRoom = newRoomObject.GetComponent<Room>();
            if (!newRoom.GetDungeonValidator().CheckIfValid()) {
                // NOTE: HAVE TO DISABLE OBJECT BECAUSE DESTROY APPEARS TO RUN AFTER START INSTEAD OF DURING
                newRoomObject.SetActive(false);
                Destroy(newRoomObject);
                connectors.Add(currentRoomEntrance);
                continue;
            }
            currentRoomCount++;
            //currentRoomEntrance.OpenEntrance();
            foreach (RoomConnectorHandler i in newRoom.GetRoomConnectors()) {
                if (Random.Range(0, 10) < 10) {
                    queue.Enqueue(i);
                }
                connectors.Add(i);
            }
        }
        
    }

    private void AddDoors() {
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