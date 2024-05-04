using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListingManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI lobbyText;
    [SerializeField] private Button joinButton;

    private LobbyData lobbyData;

    private void Start() {
        joinButton.onClick.AddListener(() => { 
            JoinLobby();
        });
    }

    private void JoinLobby() {
        LobbyHandler.Instance.JoinLobbyAsGuest(lobbyData);
    }

    public void SetLobbyListingData(LobbyData lobbyData) {
        this.lobbyData = lobbyData;
        lobbyText.text = lobbyData.Name;
    }
}