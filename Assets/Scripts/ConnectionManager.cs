using FishNet;
using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System.Collections;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    private bool isHost = false;
    private bool isConnected = false;

    private float connectTimer = 5f;

    private void Awake () {
        Instance = this;
    }

    private void ClientManager_OnAuthenticated() {
        isConnected = true;
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
            MainMenuToGameSceneHandler.LoadIntoGame(true);
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
            MainMenuToGameSceneHandler.LoadIntoGame(true);
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
            MainMenuToGameSceneHandler.LoadIntoGame(true);
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
            MainMenuToGameSceneHandler.LoadIntoGame(false);
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    private void OnEnable() {
        InstanceFinder.NetworkManager.ClientManager.OnAuthenticated += ClientManager_OnAuthenticated;
    }

    private void OnDisable() {
        InstanceFinder.NetworkManager.ClientManager.OnAuthenticated -= ClientManager_OnAuthenticated;
    }
}