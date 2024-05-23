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

    private IEnumerator StartHost() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("NetworkingScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();
        
        asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }
        
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MainMenuScene");

        SceneLoadData sld = new SceneLoadData("GameScene");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    public void OnClickJoinButton() {
        StartCoroutine(StartNetworkingScene());        
    }

    private IEnumerator StartNetworkingScene() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("NetworkingScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        MainMenuManager.Instance.ShowLobbyBrowser();
    }
}