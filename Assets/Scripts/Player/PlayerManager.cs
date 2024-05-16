using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager Instance { get; private set; }

    private Dictionary<int, Player> players = new Dictionary<int, Player>();

    private void Awake() {
        Instance = this;
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
