using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandler : MonoBehaviour {
    public static LobbyHandler Instance { get; private set; }

    [SerializeField] private LobbyManager lobbyManager;

    private float testPrintDelay = 1f;
    private float testPrintTimer = 0f;

    private void Start () {
        Instance = this;
        HeathenEngineering.SteamworksIntegration.API.Overlay.Client.EventGameLobbyJoinRequested.AddListener(OnJoinRequestAccepted);
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (testPrintTimer >= testPrintDelay) {
            testPrintTimer = 0f;
            Debug.Log("TESTING MEMBER COUNT: " + lobbyManager.MemberCount);
        }
        else {
            testPrintTimer += Time.deltaTime;
        }
    }

    public void OnLobbyCreated(LobbyData lobbyData) {
        lobbyData.Name = UserData.Me.Name + "'s lobby";
        Debug.Log("TESTING LOBBY CREATED: " + lobbyData.HexId);

    }

    public void OnLobbyJoined(LobbyData lobbyData) {
        Debug.Log("TESTING LOBBY JOINED ON LOBBY JOINED");
    }

    public void OnLobbyJoinedFailure(EChatRoomEnterResponse eChatRoomEnterResponse) {
        Debug.Log("TESTING LOBBY JOIN FAILURE " + eChatRoomEnterResponse.ToString());
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        Debug.Log("TESTING LOBBY JOINED " + lobbyData.SteamId);
        ConnectionManager.Instance.StartConnection(userData);
        SceneLoader.Load(SceneLoader.Scene.GameScene);
        lobbyManager.Join(lobbyData);
        

        Debug.Log("TESTING LOBBY JOINED FULLY SUCCESSFUL");
    }

    public void CreateLobby() {
        lobbyManager.Create();
    }

    public void InvitePlayer(UserData userData) {
        lobbyManager.Invite(userData);
    }
}