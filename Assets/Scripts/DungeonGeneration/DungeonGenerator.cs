using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : NetworkBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private Transform dungeonParent;

    [SerializeField] private LayerMask dungeonRoomOpeningColliderLayer;
    [SerializeField] private GameObject doorway;

    [SerializeField] private NavMeshSurface navMeshSurface;

    private Stack<RoomConnectorHandler> stack = new Stack<RoomConnectorHandler>();
    private HashSet<RoomConnectorHandler> connectors = new HashSet<RoomConnectorHandler>();

    public event EventHandler OnFinishDungeonGenerate;

    private int maxRooms = 50;
    private int currentRoomCount = 0;

    public override void OnStartClient() {
        base.OnStartClient();

        if (base.IsServerInitialized) {
            int seed = CreateSeed();
            GenerateDungeonObserversRpc(seed);
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void GenerateDungeonObserversRpc(int seed) {
        StartCoroutine(GenerateDungeonAsync(seed));
    }

    private IEnumerator GenerateDungeonAsync(int seed) {
        UnityEngine.Random.InitState(seed);

        yield return StartCoroutine(DungeonGenerationAsync());
        yield return StartCoroutine(ConnectRoomsAsync());
        yield return StartCoroutine(BakeNavMeshAsync());

        OnFinishDungeonGenerate?.Invoke(this, EventArgs.Empty);
    }

    // NOTE: UpdateNavMesh REBUILDS THE ENTIRE SCENE. FIND A WAY TO MAKE IT ONLY BAKE THE PART WITH THE DUNGEON
    private IEnumerator BakeNavMeshAsync() {
        AsyncOperation asyncOperation = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        while (!asyncOperation.isDone) {
            yield return null;
        }
    }

    private int CreateSeed() {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    private IEnumerator DungeonGenerationAsync() {
        RoomHandler entranceRoomScript = GetComponent<RoomHandler>();

        foreach (RoomConnectorHandler i in entranceRoomScript.GetRoomConnectors()) {
            stack.Push(i);
            connectors.Add(i);
        }
        
        while (currentRoomCount < maxRooms && stack.Count > 0) {
            RoomConnectorHandler currentRoomConnectorHandler = stack.Pop();
            ShuffleArray(rooms);
            int maxNumberOfConnections = 0;
            // NOTE: the values I assigned for the next 3 variables are only because compiler complains if I dont give them values
            Quaternion rotationOfNewRoomObject = Quaternion.identity;
            Vector3 spawnRoomPosition = Vector3.zero;
            GameObject newRoomPrefabToSpawn = null;

            RoomHandler currentParentRoomHandler = currentRoomConnectorHandler.GetComponentInParent<RoomHandler>();

            currentParentRoomHandler.DisableConnectorColliders();

            foreach (GameObject currentPrefab in rooms) {
                RoomHandler prefabRoomHandler = currentPrefab.GetComponent<RoomHandler>();
                RoomConnectorHandler[] spawnConnectorsInThisRoomPrefab = prefabRoomHandler.GetRoomSpawnConnectors();
                ShuffleArray(spawnConnectorsInThisRoomPrefab);

                foreach (RoomConnectorHandler newRoomConnectorHandler in spawnConnectorsInThisRoomPrefab) {
                    if (ValidateRoomGeneration(prefabRoomHandler, currentRoomConnectorHandler, newRoomConnectorHandler)) {
                        int numberOfConnections = 1;

                        Quaternion currentNewRoomObjectRotation = GetSpawnNewRoomObjectQuaternion(currentRoomConnectorHandler, newRoomConnectorHandler);
                        Vector3 currentNewRoomObjectPosition = GetNewRoomObjectVector(currentRoomConnectorHandler, newRoomConnectorHandler, prefabRoomHandler);

                        // this loop checks each doorway collider in the room in its current state
                        foreach (RoomConnectorHandler x in spawnConnectorsInThisRoomPrefab) {
                            if (x == newRoomConnectorHandler) {
                                continue;
                            }
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
                // NOTE: break here is so only one room is tested
                //break;
            }

            currentParentRoomHandler.EnableConnectorColliders();

            if (newRoomPrefabToSpawn) {
                // BEGIN INLINE SPAWN ROOM =========================================
                AsyncInstantiateOperation<GameObject> asyncInstantiateOperation = InstantiateAsync<GameObject>(newRoomPrefabToSpawn, spawnRoomPosition, rotationOfNewRoomObject);

                while (!asyncInstantiateOperation.isDone) {
                    yield return null;
                }

                GameObject newRoomObject = asyncInstantiateOperation.Result[0];
                newRoomObject.transform.SetParent(dungeonParent);
                RoomHandler roomHandler = newRoomObject.GetComponent<RoomHandler>();
                // END INLINE SPAWN ROOM ==========================================

                currentRoomCount++;
                bool alreadyAddedConnector = false;
                foreach (RoomConnectorHandler i in roomHandler.GetRoomConnectors()) {
                    if (alreadyAddedConnector) {
                        if (UnityEngine.Random.Range(0, 10) < 2) {
                            stack.Push(i);
                        }
                    }
                    else {
                        if (UnityEngine.Random.Range(0, 10) < 7) {
                            stack.Push(i);
                            alreadyAddedConnector = true;
                        }
                    }
                    
                    connectors.Add(i);
                }
            }
            
        }
        //Debug.Log("TESTING DUNGEON GENERATION STACK SIZE: " + stack.Count + " | CURRENT ROOM COUNT: " +  currentRoomCount);
    }

    // TODO: you might be able to optimize this by reducing potential duplicates
    private IEnumerator ConnectRoomsAsync() {
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
                AsyncInstantiateOperation<GameObject> asyncInstantiateOperation = InstantiateAsync<GameObject>(doorway, overlappingColliders[0].transform.position, overlappingColliders[0].transform.rotation);

                while (!asyncInstantiateOperation.isDone) {
                    yield return null;
                }

                GameObject newDoorwayObject = asyncInstantiateOperation.Result[0];
                newDoorwayObject.transform.SetParent(dungeonParent);
            }
        }
        Debug.Log("TESTING DUNGEON GENERATION STACK SIZE: " + stack.Count + " | CURRENT ROOM COUNT: " + currentRoomCount);
    }

    private void ShuffleArray<T>(T[] array) {
        for (int i = array.Length - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // TODO: fix this duplicate code for getting quaternion rotation
    private Quaternion GetSpawnNewRoomObjectQuaternion(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler) {
        Quaternion rotation = newRoomConnectorHandler.transform.rotation * Quaternion.Euler(0, 180, 0);
        return rotation * parentRoomConnectorHandler.transform.rotation;
    }

    private Vector3 GetNewRoomObjectVector(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler, RoomHandler prefabRoomHandler) {
        return prefabRoomHandler.GetRoomSpawnVector(parentRoomConnectorHandler, newRoomConnectorHandler);
    }

    private bool ValidateRoomGeneration(RoomHandler prefabRoomHandler, RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler) {
        if (prefabRoomHandler.GetDungeonValidator().CheckIfSpaceIsClear(parentRoomConnectorHandler, newRoomConnectorHandler)) {
            return true;
        }
        return false;
    }
}