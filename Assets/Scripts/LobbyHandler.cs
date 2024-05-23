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

    private void Start () {
        Instance = this;
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

    private void OnJoinLobbySuccess(LobbyData lobbyData) {
        if (ConnectionManager.Instance.StartConnectionAsGuest(lobbyData.Owner.user.id)) {
            //SceneHandler.Load(SceneHandler.Scenes.GameScene);
            StartCoroutine(StartGameScenes());
        }
    }

    private IEnumerator StartGameScenes() {
        UnityEngine.AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MainMenuScene");

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    private void OnAskedToLeave() {
        GameManager.Instance.QuitGame();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        //JoinLobbyAsGuest(lobbyData);
        StartCoroutine(StartNetworkingScene(lobbyData));
    }

    private IEnumerator StartNetworkingScene(LobbyData lobbyData) {
        UnityEngine.AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("NetworkingScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

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