using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

public class LobbySearcher : MonoBehaviour {

    public void FindLobbies() {
        HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.RequestLobbyList(MyCallback);
    }

    private void MyCallback(LobbyData[] lobbyDatas, bool success) {
        foreach (LobbyData data in lobbyDatas) {
            Debug.Log("TESTING MY CALLBACK: " + data.Name);
        }

    }
}