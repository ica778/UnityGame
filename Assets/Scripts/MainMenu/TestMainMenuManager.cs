using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestMainMenuManager : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private NetworkManager networkManager;

    private void Awake() {
        StartCoroutine(StartNetworkingScene());
    }

    private void Start() {
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null) {
            Debug.LogError("NetworkManager not found");
            return;
        }

        hostButton.onClick.AddListener(() => {

            StartCoroutine(StartHost());
        });

        joinButton.onClick.AddListener(() => {
            
            StartCoroutine(StartGameScenes());
        });
    }

    // NOTE: maybe duplicate code
    private IEnumerator StartHost() {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("TestMainMenu");

        SceneLoadData sld = new SceneLoadData("GameScene");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    // NOTE: maybe duplicate code
    private IEnumerator StartGameScenes() {
        networkManager.ClientManager.StartConnection();
        UnityEngine.AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("TestMainMenu");

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    // NOTE: maybe duplicate code
    private IEnumerator StartNetworkingScene() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("NetworkingSceneTest", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }
    }
}