using FishNet;
using FishNet.Managing;
using UnityEngine;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    public void OnClickHostButton() {
        Debug.LogError("TODO: FEATURE NOT IMPLEMENTED YET, STILL HAVE TO MAKE HOSTING WORK FOR STEAM");
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

        SceneHandler.Instance.UnloadMainMenuScene();
        
    }
}