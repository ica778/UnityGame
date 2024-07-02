using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

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
        GameSceneManager.Instance.QuitGame();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        JoinLobbyAsGuest(lobbyData);
    }

    private void OnLobbyCreateSuccess(LobbyData lobbyData) {
        lobbyData.Name = lobbyData.Owner.user.Name + "'s Lobby";
        ConnectionManager.Instance.StartGameAsHostSteam();
    }

    // This is what you use to join game, dont need to go into ConnectionManager, just call this function
    public void JoinLobbyAsGuest(LobbyData lobbyData) {
        lobbyManager.Join(lobbyData);
    }

    // This is what you use to create a game as host. Dont need to go into ConnectionManager, just call this function
    public void CreateLobby() {
        lobbyManager.Create();
    }

    public void InvitePlayer(UserData userData) {
        lobbyManager.Invite(userData);
    }

    public LobbyManager GetLobbyManager() {
        return lobbyManager;
    }

    private void KickEveryone() {
        if (lobbyManager.IsPlayerOwner) {
            foreach (LobbyMemberData lobbyMemberData in lobbyManager.Members) {
                if (!lobbyMemberData.IsOwner) {
                    Debug.Log("TESTING KICK USER: " + lobbyMemberData.user.Name);
                    lobbyMemberData.Kick();
                }
            }
        }
    }

    public void DestroySelf() {
        KickEveryone();

        lobbyManager.Leave();
        Destroy(gameObject);
        Instance = null;
    }

    public void Leave() {
        lobbyManager.Leave();
    }

}