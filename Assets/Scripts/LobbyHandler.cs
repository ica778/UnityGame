using FishNet.Managing.Scened;
using FishNet;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static HeathenEngineering.SteamworksIntegration.SteamSettings;
using System.ComponentModel;

public class LobbyHandler : MonoBehaviour {
    public static LobbyHandler Instance { get; private set; }

    [SerializeField] private LobbyManager lobbyManager;

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        HeathenEngineering.SteamworksIntegration.API.Overlay.Client.EventGameLobbyJoinRequested.AddListener(OnJoinRequestAccepted);
        lobbyManager.evtAskedToLeave.AddListener(OnAskedToLeave);
        lobbyManager.evtEnterSuccess.AddListener(OnJoinLobbySuccess);
        lobbyManager.evtCreated.AddListener(OnLobbyCreateSuccess);
    }

    public void Testing() {
        foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
            if (!lobbyMemberData.IsOwner) {
                Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                lobbyMemberData.Kick();
            }
        }
    }

    // TODO: add proper error handling for hosting and joining games
    private void OnJoinLobbySuccess(LobbyData lobbyData) {
        ConnectionManager.Instance.StartGameAsClientSteam(lobbyData.Owner.user.id);
    }

    private void OnAskedToLeave() {
        GameManager.Instance.QuitGame();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        JoinLobbyAsGuest(lobbyData);
    }

    private void OnLobbyCreateSuccess(LobbyData lobbyData) {
        lobbyData.Name = lobbyData.Owner.user.Name + "'s Lobby";
    }

    // This is what you use to join game, dont need to go into ConnectionManager, just call this function
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