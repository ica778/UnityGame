using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private GameObject playerObject;

    public void OnClickHostButton() {
        StartCoroutine(StartHost());
    }

    // NOTE: maybe duplicate code
    private IEnumerator StartHost() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();

        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }
        
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MainMenuScene");

        SceneLoadData sld = new SceneLoadData("GameScene");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    public void OnClickJoinButton() {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }
}