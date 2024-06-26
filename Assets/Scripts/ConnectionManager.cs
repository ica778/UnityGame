using FishNet;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    public event EventHandler OnNetworkTimeout;

    private bool isHost = false;
    private bool isConnected = false;

    private float connectTimer = 5f;

    private void Awake () {
        Instance = this;
    }

    private IEnumerator StartGameAsHostSteamAsync() {
        float timer = connectTimer;

        fishySteamworks.StartConnection(true);
        fishySteamworks.StartConnection(false);

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            isHost = true;
            ClientSideGameSceneManager.Instance.LoadIntoGame(true);
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    // start server and join as host
    public void StartGameAsHostSteam() {
        StartCoroutine(StartGameAsHostSteamAsync());
    }

    private IEnumerator StartGameAsClientSteamAsync(CSteamID hostCSteamID) {
        float timer = connectTimer;
        UserData hostUser = UserData.Get(hostCSteamID);

        if (!hostUser.IsValid) {
            Debug.LogError("TESTING HOST USER IS NOT VALID");
        }

        fishySteamworks.SetClientAddress(hostCSteamID.ToString());
        fishySteamworks.StartConnection(false);

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            ClientSideGameSceneManager.Instance.LoadIntoGame(false);
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    // join server as guest using host CSteamID
    public void StartGameAsClientSteam(CSteamID hostCSteamID) {
        StartCoroutine(StartGameAsClientSteamAsync(hostCSteamID));
    }

    public void Disconnect() {
        isConnected = false;
        if (isHost) {
            fishySteamworks.Shutdown();
        }
        else {
            fishySteamworks.StopConnection(false);
        }
        isHost = false;
    }

    // NOTE: this is for testing multiplayer without having to use steam
    public void StartGameAsHostOffline() {
        StartCoroutine(StartGameAsHostOfflineAsync());
    }

    private IEnumerator StartGameAsHostOfflineAsync() {
        float timer = connectTimer;

        InstanceFinder.NetworkManager.ServerManager.StartConnection();
        InstanceFinder.NetworkManager.ClientManager.StartConnection();

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            isHost = true;
            ClientSideGameSceneManager.Instance.LoadIntoGame(true);
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    public void StartGameAsClientOffline() {
        StartCoroutine(StartGameAsClientOfflineAsync());
    }

    private IEnumerator StartGameAsClientOfflineAsync() {
        float timer = connectTimer;

        InstanceFinder.NetworkManager.ClientManager.StartConnection();

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            ClientSideGameSceneManager.Instance.LoadIntoGame(false);
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    private void ClientManager_OnAuthenticated() {
        isConnected = true;
    }

    // NOTE: FOR SOME REASON CANT USE OnClientTimeOut SO I HAVE TO USE THIS
    private void ClientManager_OnClientConnectionState(FishNet.Transporting.ClientConnectionStateArgs obj) {
        if (isConnected && obj.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped) {
            LobbyHandler.Instance.Leave();
            Disconnect();
            ClientSideGameSceneManager.Instance.QuitGameBackToMainMenu();
            OnNetworkTimeout?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnEnable() {
        InstanceFinder.NetworkManager.ClientManager.OnAuthenticated += ClientManager_OnAuthenticated;
        InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
    }

    private void OnDisable() {
        InstanceFinder.NetworkManager.ClientManager.OnAuthenticated -= ClientManager_OnAuthenticated;
        InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
    }
}