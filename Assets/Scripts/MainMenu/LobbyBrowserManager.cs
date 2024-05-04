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

    private void ClearLobbyListings() {
        lobbies.Clear();

        for (int i = scrollViewContentBox.childCount - 1; i >= 0; i--) {
            Destroy(scrollViewContentBox.GetChild(i).gameObject);
        }
    }

    public void FindLobbies() {
        HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.RequestLobbyList(RequestLobbyListCallback);
    }

    public void OnClickBackButton() {
        MainMenuManager.Instance.ShowDefaultMenu();
        ClearLobbyListings();
    }
}