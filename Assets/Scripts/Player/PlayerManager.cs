using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    private Dictionary<NetworkConnection, Player> players = new();

    private void Awake() {
        Instance = this;
    }

    public override void OnStartClient() {
        NetworkConnection conn = base.LocalConnection;

        SpawnPlayerServerRpc(conn);
    }
    
    // TODO: CONFIRM THIS IS THE BEST WAY OF SPAWNING PLAYER OBJECTS IN THE CORRECT SCENE AND ALSO NEED TO SET OWNER
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(NetworkConnection conn) {

        GameObject newPlayer = Instantiate(playerPrefab);
        base.NetworkManager.ServerManager.Spawn(newPlayer, conn);

        //MovePlayersToCorrectSceneObserversRpc(newPlayer);
    }

    [ObserversRpc]
    private void MovePlayersToCorrectSceneObserversRpc(GameObject newPlayer) {
        foreach (Player currentPlayer in players.Values) {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(currentPlayer.gameObject, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePersistentObjectsScene"));
        }
    }
    
    public bool HasPlayer(NetworkConnection conn) {
        return players.ContainsKey(conn);
    }

    public Player GetPlayer(NetworkConnection conn) {
        return players[conn];
    }

    public void AddPlayer(NetworkConnection conn, Player player) {
        players.Add(conn, player);
    }

    public void RemovePlayer(NetworkConnection conn) {
        players.Remove(conn);
    }
}
