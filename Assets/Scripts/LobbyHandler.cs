using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

public class LobbyHandler : MonoBehaviour {
    public static LobbyHandler Instance { get; private set; }

    [SerializeField] private LobbyManager lobbyManager;

    private void Start () {
        Instance = this;
        HeathenEngineering.SteamworksIntegration.API.Overlay.Client.EventGameLobbyJoinRequested.AddListener(OnJoinRequestAccepted);
        lobbyManager.evtAskedToLeave.AddListener(OnAskedToLeave);
        lobbyManager.evtEnterSuccess.AddListener(OnJoinLobbySuccess);
        DontDestroyOnLoad(gameObject);
    }

    public void Testing() {
        foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
            if (!lobbyMemberData.IsOwner) {
                Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                lobbyMemberData.Kick();
            }
        }
    }

    private void OnJoinLobbySuccess(LobbyData lobbyData) {
        if (ConnectionManager.Instance.StartConnectionAsGuest(lobbyManager.Lobby.Owner.user)) {
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        }
    }

    private void OnAskedToLeave() {
        GameManager.Instance.QuitGame();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        JoinLobbyAsGuest(lobbyData);
    }
    
    // This is what you use to join game, dont need to go into ConnectionManager
    public void JoinLobbyAsGuest(LobbyData lobbyData) {
        lobbyManager.Join(lobbyData);
    }

    public void CreateLobby() {
        lobbyManager.Create();
    }

    public void InvitePlayer(UserData userData) {
        lobbyManager.Lobby.ClearKickList();
        lobbyManager.Invite(userData);
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