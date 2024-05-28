using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {
    [SerializeField] private GameObject player;

    public static PlayerManager Instance { get; private set; }

    private Dictionary<int, Player> players = new Dictionary<int, Player>();

    // TODO: make spawning happen on server
    private void Awake() {
        Instance = this;      
    }

    private void Start() {
        StartCoroutine(SpawnThePlayer());
    }

    private IEnumerator SpawnThePlayer() {
        Debug.Log("TESTING WAITING");
        yield return new WaitForSeconds(5);
        Debug.Log("TESTING FINISHED WAIT");

        NetworkConnection conn = InstanceFinder.ClientManager.Connection;
        SpawnPlayerServerRpc(conn);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(NetworkConnection conn) {
        GameObject newPlayer = Instantiate(player);
        InstanceFinder.NetworkManager.ServerManager.Spawn(newPlayer, conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("PlayerScene"));
        //InstanceFinder.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("PlayerScene"));
    }

    public bool HasPlayer(int playerId) {
        return players.ContainsKey(playerId);
    }

    public Player GetPlayer(int playerId) {
        return players[playerId];
    }

    public void AddPlayer(int playerId, Player player) {
        if (players.ContainsKey(playerId)) {
            return;
        }
        players.Add(playerId, player);
    }

    public void RemovePlayer(int playerId) {
        players.Remove(playerId);
    }

    public Dictionary<int, Player> GetPlayersList() {
        return players;
    }

}
