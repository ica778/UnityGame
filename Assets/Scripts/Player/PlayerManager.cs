using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    private Dictionary<int, Player> players = new Dictionary<int, Player>();

    private void Awake() {
        Instance = this;
    }

    public override void OnStartClient() {
        NetworkConnection conn = base.ClientManager.Connection;

        SpawnPlayerServerRpc(conn);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(NetworkConnection conn) {

        GameObject newPlayer = Instantiate(playerPrefab);
        base.NetworkManager.ServerManager.Spawn(newPlayer, conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePersistentObjectsScene"));

        MovePlayerToCorrectSceneObserversRpc(newPlayer);
    }

    [ObserversRpc]
    private void MovePlayerToCorrectSceneObserversRpc(GameObject newPlayer) {
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newPlayer, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePersistentObjectsScene"));
    }
    
    public bool HasPlayer(int playerId) {
        return players.ContainsKey(playerId);
    }

    public Player GetPlayer(int playerId) {
        return players[playerId];
    }

    public void AddPlayer(int playerId, Player player) {
        players.Add(playerId, player);
    }

    public void RemovePlayer(int playerId) {
        players.Remove(playerId);
    }

    public Dictionary<int, Player> GetPlayersList() {
        return players;
    }

}
