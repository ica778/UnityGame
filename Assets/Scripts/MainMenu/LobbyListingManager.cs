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
        LobbyHandler.Instance.JoinLobbyAsGuest(lobbyData, UserData.Get());
    }

    public void SetText(string text) {
        lobbyText.text = text;
    }

    public void SetLobbyData(LobbyData lobbyData) {
        this.lobbyData = lobbyData;
    }
}