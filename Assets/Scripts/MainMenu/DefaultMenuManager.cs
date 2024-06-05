using FishNet;
using FishNet.Managing;
using System.Collections;
using UnityEngine;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    public void OnClickHostButton() {
        Debug.LogError("TODO: FEATURE NOT IMPLEMENTED YET, STILL HAVE TO MAKE HOSTING WORK FOR STEAM");
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();

        //SceneHandler.Instance.LoadIntoGame();
    }

    public void OnClickJoinButton() {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }

    // NOTE: this is for testing multiplayer without steam
    public void OnClickTestHostButton() {
        ConnectionManager.Instance.StartGameAsHostOffline();
    }

    // NOTE: this is for testing multiplayer without steam
    public void OnClickTestJoinButton() {
        ConnectionManager.Instance.StartGameAsClientOffline();
        //StartCoroutine(TestingStartGameAfterDelay());
    }

    private IEnumerator TestingStartGameAfterDelay() {
        yield return new WaitForSeconds(5);
        ConnectionManager.Instance.StartGameAsClientOffline();
    }
}