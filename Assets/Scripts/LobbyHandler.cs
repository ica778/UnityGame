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
        lobbyManager.evtAskedToLeave.AddListener(OnAskedToLeave);
        lobbyManager.evtCreated.AddListener(TESTINGPRINT);
        DontDestroyOnLoad(gameObject);
    }

    public void OnAskedToLeave() {
        Debug.Log("TESTING ASKED TO LEAVE ==================================");
        lobbyManager.Lobby.Leave();
        lobbyManager.Lobby = default;
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);

    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        ConnectionManager.Instance.ConnectToServer(lobbyData, userData);
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void TESTINGPRINT(LobbyData lobbyData) {
        Debug.Log("TESTING LOBBY JOINED");
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

    public void KickSelf() {
        foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
            if (!lobbyMemberData.IsOwner) {
                Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                lobbyMemberData.Kick();
            }
        }
    }

    public void DestroySelf() {
        Debug.Log("TESTING EXIT ====================================: ");
        
        if (lobbyManager.IsPlayerOwner) {
            foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
                if (lobbyMemberData.IsOwner) {
                    Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                    lobbyMemberData.Kick();
                }
                //lobbyMemberData.Kick();
            }
        }
        /*
        lobbyManager.Lobby.ClearKickList();
        lobbyManager.Leave();
        lobbyManager.Lobby = default;
        Destroy(gameObject);
        Instance = null;
        */
    }

}