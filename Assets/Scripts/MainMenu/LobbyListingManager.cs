using HeathenEngineering.SteamworksIntegration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListingManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCapacity;
    [SerializeField] private TextMeshProUGUI ping;
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
        lobbyName.SetText(lobbyData.Name);
        lobbyCapacity.SetText(lobbyData.MemberCount + "/" + lobbyData.MaxMembers);
        ping.SetText("69");
    }
}