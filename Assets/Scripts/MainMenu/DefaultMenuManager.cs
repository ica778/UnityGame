using UnityEngine;

public class DefaultMenuManager : MonoBehaviour {

    public void OnClickHostButton() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void OnClickJoinButton() {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }
}