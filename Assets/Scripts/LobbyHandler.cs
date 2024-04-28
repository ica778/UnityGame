using HeathenEngineering.SteamworksIntegration;
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
        DontDestroyOnLoad(gameObject);
    }

    public void OnLobbyCreated(LobbyData lobbyData) {
        lobbyData.Name = UserData.Me.Name + "'s lobby";
    }

    public void OnLobbyJoined(LobbyData lobbyData) {
        Debug.Log("TESTING LOBBY JOINED ON LOBBY JOINED =====================================");
    }

    public void OnLobbyJoinedFailure(EChatRoomEnterResponse eChatRoomEnterResponse) {
        Debug.LogError("LOBBY JOIN FAILURE " + eChatRoomEnterResponse.ToString());
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

    public void KickPlayer(UserData userData) {
        lobbyManager.KickMember(userData);
    }

    public bool IsHost() {
        return lobbyManager.IsPlayerOwner;
    }

    public void DestroySelf() {
        if (lobbyManager.IsPlayerOwner) {
            foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
                Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                KickPlayer(lobbyMemberData.user);
            }
        }
        lobbyManager.Leave();
        Destroy(gameObject);
        Instance = null;
    }

}