using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    private string hostHex;
    private bool isHost = false;

    private void Start () {
        Instance = this;
    }

    private void StartHost() {
        var user = UserData.Get();
        hostHex = user.ToString();

        fishySteamworks.StartConnection(true);
        fishySteamworks.StartConnection(false);

    }

    private void StartConnection(UserData userData) {
        hostHex = userData.ToString();
        var hostUser = UserData.Get(hostHex);

        if (!hostUser.IsValid) {
            Debug.Log("TESTING HOST USER IS NOT VALID");
        }

        fishySteamworks.SetClientAddress(hostUser.id.ToString());
        fishySteamworks.StartConnection(false);
    }

    private void StopConnection() {
        if (isHost) {
            fishySteamworks.Shutdown();
        }
        else {
            fishySteamworks.StopConnection(false);
        }
        isHost = false;
        hostHex = null;
    }

    public void ConnectAsHost() {
        StartHost();
        isHost = true;
        LobbyHandler.Instance.CreateLobby();
    }

    public void ConnectToServer(LobbyData lobbyData, UserData userData) {
        StartConnection(userData);
        LobbyHandler.Instance.JoinLobby(lobbyData);
    }

    public void DisconnectFromServer() {
        LobbyHandler.Instance.DestroySelf();
        StopConnection();
        
    }

}