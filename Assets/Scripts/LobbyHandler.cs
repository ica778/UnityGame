using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandler : MonoBehaviour {
    public static LobbyHandler Instance { get; private set; }

    [SerializeField] private LobbyManager lobbyManager;

    private void Start () {
        Instance = this;
        HeathenEngineering.SteamworksIntegration.API.Overlay.Client.EventGameLobbyJoinRequested.AddListener(OnJoinRequestAccepted);
        HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.EventLobbyAskedToLeave.AddListener(OnAskedToLeave);
        DontDestroyOnLoad(gameObject);
    }

    private void OnAskedToLeave(LobbyData lobbyData) {
        GameManager.Instance.QuitGame();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        ConnectionManager.Instance.ConnectToServer(lobbyData, userData);
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void JoinLobby(LobbyData lobbyData) {
        lobbyManager.Join(lobbyData);
    }

    public void CreateLobby() {
        lobbyManager.Create();
    }

    public void InvitePlayer(UserData userData) {
        lobbyManager.Invite(userData);
    }

    public bool IsHost() {
        return lobbyManager.IsPlayerOwner;
    }

    public LobbyManager GetLobbyManager() {
        return lobbyManager;
    }

    public void DestroySelf() {
        if (lobbyManager.IsPlayerOwner) {
            foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
                if (!lobbyMemberData.IsOwner) {
                    Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                    lobbyMemberData.Kick();
                }
            }
        }
        lobbyManager.Leave();
        Destroy(gameObject);
        Instance = null;
    }

}