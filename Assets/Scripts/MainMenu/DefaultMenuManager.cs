using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    // NOTE: this is for testing multiplayer without steam
    [SerializeField] private NetworkManager offlineNetworkManager;

    public void OnClickHostButton() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();

        SceneHandler.Instance.LoadFromMainMenuToGameScene();
    }

    public void OnClickJoinButton() {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }

    // NOTE: this is for testing multiplayer without steam
    public void OnClickTestHostButton() {
        ConnectionManager.Instance.StartHostOffline();

        SceneHandler.Instance.LoadFromMainMenuToGameScene();
    }

    // NOTE: this is for testing multiplayer without steam
    public void OnClickTestJoinButton() {
        ConnectionManager.Instance.StartConnectionAsGuestOffline();

        SceneHandler.Instance.LoadFromMainMenuToGameScene();
    }
}