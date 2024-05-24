using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    public void OnClickHostButton() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();

        SceneHandler.Instance.LoadFromMainMenuToGameScene();
    }

    public void OnClickJoinButton() {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }
}