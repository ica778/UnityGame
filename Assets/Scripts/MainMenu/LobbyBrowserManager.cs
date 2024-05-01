using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyBrowserManager : MonoBehaviour {
    private List<LobbyData> lobbies = new List<LobbyData>();

    private void OnEnable() {
        FindLobbies();
    }

    private void RequestLobbyListCallback(LobbyData[] lobbyDatas, bool success) {
        foreach (LobbyData lobbyData in lobbyDatas) {
            if (!success) {
                Debug.Log("TESTING MY CALLBACK: " + lobbyData.Name);
                lobbies.Add(lobbyData);
            }
        }
    }

    public void FindLobbies() {
        HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.RequestLobbyList(RequestLobbyListCallback);
    }

    public void OnClickBackButton() {
        MainMenuManager.Instance.ShowDefaultMenu();
    }
}