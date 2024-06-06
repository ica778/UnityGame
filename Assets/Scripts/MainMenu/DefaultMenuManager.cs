using FishNet;
using FishNet.Managing;
using System.Collections;
using UnityEngine;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    public void OnClickHostButton() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartGameAsHostSteam();
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