using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    private bool isHost = false;

    private void Start () {
        Instance = this;
    }

    // start server and join as host
    public bool StartHost() {
        UserData user = UserData.Get();

        if (fishySteamworks.StartConnection(true)) {
            if (fishySteamworks.StartConnection(false)) {
                isHost = true;
                LobbyHandler.Instance.SetLobbyGameServer(user.id);
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
}