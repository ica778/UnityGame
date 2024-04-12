using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : NetworkBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    [SerializeField] private LayerMask dungeonRoomOpeningColliderLayer;
    [SerializeField] private GameObject doorway;

    private Stack<RoomConnectorHandler> stack = new Stack<RoomConnectorHandler>();
    private HashSet<RoomConnectorHandler> connectors = new HashSet<RoomConnectorHandler>(); 

    private int maxRooms = 50;
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
            stack.Push(i);
            connectors.Add(i);
        }
        
        while (currentRoomCount < maxRooms && stack.Count > 0) {
            RoomConnectorHandler currentRoomConnectorHandler = stack.Pop();
            ShuffleRooms(rooms);
            int maxNumberOfConnections = 0;
            // NOTE: the values I assigned for the next 3 variables are only because compiler complains if I dont give them values
            Quaternion rotationOfNewRoomObject = Quaternion.identity;
            Vector3 spawnRoomPosition = Vector3.zero;
            GameObject newRoomPrefabToSpawn = null;

            RoomHandler currentParentRoomHandler = currentRoomConnectorHandler.GetComponentInParent<RoomHandler>();
            currentParentRoomHandler.DisableConnectorColliders();

            foreach (GameObject currentPrefab in rooms) {
                RoomHandler prefabRoomHandler = currentPrefab.GetComponent<RoomHandler>();
                RoomConnectorHandler[] connectorsInThisRoomPrefab = prefabRoomHandler.GetRoomConnectors();
                ShuffleRooms(connectorsInThisRoomPrefab);

                foreach (RoomConnectorHandler newRoomConnectorHandler in connectorsInThisRoomPrefab) {
                    if (ValidateRoomGeneration(prefabRoomHandler, currentRoomConnectorHandler, newRoomConnectorHandler)) {
                        int numberOfConnections = 1;

                        Quaternion currentNewRoomObjectRotation = GetSpawnNewRoomObjectQuaternion(currentRoomConnectorHandler, newRoomConnectorHandler, prefabRoomHandler);
                        Vector3 currentNewRoomObjectPosition = GetNewRoomObjectVector(currentRoomConnectorHandler, newRoomConnectorHandler, prefabRoomHandler);

                        // this loop checks each doorway collider in the room in its current state
                        foreach (RoomConnectorHandler x in connectorsInThisRoomPrefab) {
                            BoxCollider collider = x.GetDoorwayCollider();
                            
                            Vector3 center = currentNewRoomObjectPosition + currentNewRoomObjectRotation * collider.transform.position;
                            Vector3 extents = collider.bounds.extents;
                            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, Quaternion.identity, dungeonRoomOpeningColliderLayer);
                            if (overlappingColliders.Length > 0) {
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
                    // NOTE: break here is for testing so that only one doorway is tested
                    //break;
                }
            }

            currentParentRoomHandler.EnableConnectorColliders();

            if (newRoomPrefabToSpawn) {
                RoomHandler roomHandler = SpawnRoomObject(spawnRoomPosition, rotationOfNewRoomObject, newRoomPrefabToSpawn);
                currentRoomCount++;
                bool alreadyAddedConnector = false;
                foreach (RoomConnectorHandler i in roomHandler.GetRoomConnectors()) {
                    if (alreadyAddedConnector) {
                        if (Random.Range(0, 10) < 3) {
                            stack.Push(i);
                        }
                    }
                    else {
                        if (Random.Range(0, 10) < 7) {
                            stack.Push(i);
                            alreadyAddedConnector = true;
                        }
                    }
                    
                    connectors.Add(i);
                }
            }
            
        }
    }

    private void ShuffleRooms<T>(T[] array) {
        for (int i = array.Length - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
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