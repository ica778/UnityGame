using FishNet.Object;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class DungeonGenerator : NetworkBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    [SerializeField] private LayerMask dungeonRoomOpeningColliderLayer;
    [SerializeField] private GameObject doorway;

    private Queue<RoomConnectorHandler> queue = new Queue<RoomConnectorHandler>();
    private HashSet<RoomConnectorHandler> connectors = new HashSet<RoomConnectorHandler>(); 

    private int maxRooms = 1;
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
            RoomConnectorHandler currentRoomConnectorHandler = queue.Dequeue();
            ShuffleRooms();
            RoomHandler roomHandler = null;
            int maxNumberOfConnections = 0;
            Quaternion rotationOfNewRoomObject = Quaternion.identity;
            Vector3 spawnRoomPosition = Vector3.zero;
            GameObject newRoomPrefabToSpawn = null;

            foreach (GameObject currentPrefab in rooms) {
                RoomHandler prefabRoomHandler = currentPrefab.GetComponent<RoomHandler>();
                RoomConnectorHandler[] openingsInThisRoomPrefab = prefabRoomHandler.GetRoomConnectors();

                foreach (RoomConnectorHandler newRoomConnectorHandler in openingsInThisRoomPrefab) {
                    if (ValidateRoomGeneration(prefabRoomHandler, currentRoomConnectorHandler, newRoomConnectorHandler)) {
                        int numberOfConnections = 0;

                        Quaternion currentNewRoomObjectRotation = GetSpawnNewRoomObjectQuaternion(currentRoomConnectorHandler, newRoomConnectorHandler, prefabRoomHandler);
                        Vector3 currentNewRoomObjectPosition = GetNewRoomObjectVector(currentRoomConnectorHandler, newRoomConnectorHandler, prefabRoomHandler);

                        // this loop checks each doorway collider in the room in its current state
                        foreach (RoomConnectorHandler x in openingsInThisRoomPrefab) {
                            BoxCollider collider = x.GetDoorwayCollider();
                            
                            Vector3 center = currentNewRoomObjectPosition + currentNewRoomObjectRotation * collider.transform.position;
                            Vector3 extents = collider.bounds.extents;
                            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, Quaternion.identity, dungeonRoomOpeningColliderLayer);
                            if (overlappingColliders.Length > 0) {
                                Debug.Log("TESTING FOUND OVERLAP " + center);
                                numberOfConnections++;
                            }
                        }

                        if (numberOfConnections > maxNumberOfConnections) {
                            maxNumberOfConnections = numberOfConnections;
                            rotationOfNewRoomObject = currentNewRoomObjectRotation;
                            spawnRoomPosition = currentNewRoomObjectPosition;
                            newRoomPrefabToSpawn = currentPrefab;
                        }
                    }
                    Debug.Log("TESTING COLLIDER " + maxNumberOfConnections);
                    // NOTE: break here is for testing so that only one doorway is tested
                    //break;
                }
            }

            if (newRoomPrefabToSpawn) {
                roomHandler = SpawnRoomObject(spawnRoomPosition, rotationOfNewRoomObject, newRoomPrefabToSpawn);
                currentRoomCount++;
                foreach (RoomConnectorHandler i in roomHandler.GetRoomConnectors()) {
                    if (Random.Range(0, 10) < 10) {
                        queue.Enqueue(i);
                    }
                    connectors.Add(i);
                }
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

    private Quaternion GetSpawnNewRoomObjectQuaternion(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler, RoomHandler prefabRoomHandler) {
        Quaternion rotationOfNewRoomObject = Quaternion.Inverse(parentRoomConnectorHandler.transform.rotation) * newRoomConnectorHandler.transform.rotation;

        if (rotationOfNewRoomObject.eulerAngles.y == 0 || rotationOfNewRoomObject.eulerAngles.y == 180) {
            rotationOfNewRoomObject *= Quaternion.Euler(0, 180, 0);
        }
        return rotationOfNewRoomObject;
    }

    private Vector3 GetNewRoomObjectVector(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler, RoomHandler prefabRoomHandler) {
        return prefabRoomHandler.GetRoomSpawnVector(parentRoomConnectorHandler, newRoomConnectorHandler);
    }

    private RoomHandler SpawnRoomObject(Vector3 spawnRoomPosition, Quaternion rotationOfNewRoomObject, GameObject prefab) {
        GameObject newRoomObject = Instantiate(
            prefab,
            spawnRoomPosition,
            rotationOfNewRoomObject
            );
        RoomHandler newRoomHandler = newRoomObject.GetComponent<RoomHandler>();
        return newRoomHandler;
    }

    private bool ValidateRoomGeneration(RoomHandler prefabRoomHandler, RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler) {
        if (prefabRoomHandler.GetDungeonValidator().CheckIfSpaceIsClear(parentRoomConnectorHandler, newRoomConnectorHandler)) {
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