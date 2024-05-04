using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBrowserManager : MonoBehaviour {
    private List<LobbyData> lobbies = new List<LobbyData>();

    [SerializeField] private GameObject lobbyListingPrefab;
    [SerializeField] private RectTransform scrollViewContentBox;

    private float lobbyListingHeight = 100f;
    private float lobbyListingSpawnHeight;

    private void OnEnable() {
        FindLobbies();
    }

    private void RequestLobbyListCallback(LobbyData[] lobbyDatas, bool success) {
        foreach (LobbyData lobbyData in lobbyDatas) {
            if (!success) {
                lobbies.Add(lobbyData);
            }
        }
        PopulateLobbyList();
    }

    private void PopulateLobbyList() {
        Vector3 newSizeDelta = scrollViewContentBox.sizeDelta;
        newSizeDelta.y = lobbyListingHeight * lobbies.Count;
        scrollViewContentBox.sizeDelta = newSizeDelta;

        lobbyListingSpawnHeight = ((lobbyListingHeight * lobbies.Count) / 2f) - (lobbyListingHeight / 2f);

        foreach (LobbyData lobbyData in lobbies) {
            GameObject newLobbyListing = Instantiate(lobbyListingPrefab, scrollViewContentBox.transform);
            newLobbyListing.transform.localPosition += new Vector3(0, lobbyListingSpawnHeight, 0);
            lobbyListingSpawnHeight -= lobbyListingHeight;

            LobbyListingManager lobbyListingManager = newLobbyListing.GetComponent<LobbyListingManager>();
            lobbyListingManager.SetLobbyListingData(lobbyData);
        }

    }

    public void FindLobbies() {
        //HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.AddRequestLobbyListFilterSlotsAvailable
        HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.RequestLobbyList(RequestLobbyListCallback);
    }

    public void OnClickBackButton() {
        MainMenuManager.Instance.ShowDefaultMenu();
    }
}