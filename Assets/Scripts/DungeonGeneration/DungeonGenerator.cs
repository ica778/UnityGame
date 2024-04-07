using FishNet.Object;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : NetworkBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    [SerializeField] private LayerMask dungeonRoomOpeningColliderLayer;
    [SerializeField] private GameObject doorway;

    private Queue<RoomConnectorHandler> queue = new Queue<RoomConnectorHandler>();
    private HashSet<RoomConnectorHandler> connectors = new HashSet<RoomConnectorHandler>(); 

    private int maxRooms = 3;
    private int currentRoomCount = 0;


    private void Start () {

    }

    public override void OnStartClient() {
        base.OnStartClient();

        if (IsServer) {
            int seed = CreateSeed();
            GenerateDungeon(seed);
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void GenerateDungeon(int seed) {
        Random.InitState(seed);

        DungeonGeneration();
        ConnectRooms();
    }

    private int CreateSeed() {
        return Random.Range(int.MinValue, int.MaxValue);
    }

    private void DungeonGeneration() {
        GameObject entrance = Instantiate(entranceRoom, transform.position, Quaternion.identity);
        RoomHandler entranceRoomScript = entrance.GetComponent<RoomHandler>();

        foreach (RoomConnectorHandler i in entranceRoomScript.GetRoomConnectors()) {
            queue.Enqueue(i);
            connectors.Add(i);
        }
        
        while (currentRoomCount < maxRooms && queue.Count > 0) {
            RoomConnectorHandler currentRoomEntrance = queue.Dequeue();
            //ShuffleRooms();
            RoomHandler roomHandler = null;
            foreach (GameObject currentPrefab in rooms) {
                RoomHandler prefabRoomHandler = currentPrefab.GetComponent<RoomHandler>();
                if (ValidateRoomGeneration(currentPrefab, prefabRoomHandler, currentRoomEntrance)) {
                    roomHandler = SpawnRoomObject(currentRoomEntrance, prefabRoomHandler, currentPrefab);
                    break;
                }
            }
            if (!roomHandler) {
                continue;
            }

            currentRoomCount++;

            foreach (RoomConnectorHandler i in roomHandler.GetRoomConnectors()) {
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

    private RoomHandler SpawnRoomObject(RoomConnectorHandler origin, RoomHandler prefabRoomHandler, GameObject prefab) {
        Vector3 spawnRoomPosition = prefabRoomHandler.GetRoomSpawnVector(origin);

        GameObject newRoomObject = Instantiate(
            prefab,
            spawnRoomPosition,
            origin.transform.rotation
            );
        RoomHandler newRoomHandler = newRoomObject.GetComponent<RoomHandler>();
        return newRoomHandler;
    }

    private bool ValidateRoomGeneration(GameObject roomPrefab, RoomHandler prefabRoomHandler, RoomConnectorHandler roomSpawnEntrance) {
        if (prefabRoomHandler.GetDungeonValidator().CheckIfSpaceIsClear(roomPrefab, roomSpawnEntrance)) {
            return true;
        }
        return false;
    }

    // TODO: could probably optimize this by reducing potential duplicates
    private void ConnectRooms() {
        foreach (RoomConnectorHandler connector in connectors) {
            BoxCollider collider = connector.GetDoorwayCollider();
            Vector3 center = collider.transform.position;
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