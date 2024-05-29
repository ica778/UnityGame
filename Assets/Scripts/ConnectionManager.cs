using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System.Collections;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    // TODO: this serialized field is for testing multiplayer without having to use steam
    [SerializeField] private NetworkManager offlineNetworkManager;

    private bool isHost = false;
    private bool isConnected = false;

    private float connectTimer = 5f;

    private void Awake () {
        Instance = this;

        offlineNetworkManager.ClientManager.OnAuthenticated += ClientManager_OnAuthenticated;
    }

    private void ClientManager_OnAuthenticated() {
        isConnected = true;
    }

    // start server and join as host
    public bool StartHost() {
        if (fishySteamworks.StartConnection(true)) {
            if (fishySteamworks.StartConnection(false)) {
                isHost = true;
                return true;
            }
        }

        return false;
    }

    // join server as guest using host CSteamID
    public bool StartConnectionAsGuest(CSteamID hostCSteamID) {
        UserData hostUser = UserData.Get(hostCSteamID);

        if (!hostUser.IsValid) {
            Debug.LogError("TESTING HOST USER IS NOT VALID");
        }
        fishySteamworks.SetClientAddress(hostCSteamID.ToString());
        return fishySteamworks.StartConnection(false);
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

        offlineNetworkManager.ServerManager.StartConnection();
        offlineNetworkManager.ClientManager.StartConnection();

        while (!isConnected || timer <= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            isHost = true;
            SceneHandler.Instance.LoadIntoGame();
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

        offlineNetworkManager.ClientManager.StartConnection();

        while (!isConnected || timer <= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            SceneHandler.Instance.LoadIntoGame();
        }
        else {
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }
}