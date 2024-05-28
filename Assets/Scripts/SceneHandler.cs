using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SceneHandler : MonoBehaviour {
    public enum SceneName {
        None,
        BootStrapScene,
        MainMenuScene,
        PlayerScene,
        GameScene,
    }

    public static SceneHandler Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private Scene GetScene(SceneName scene) {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene.ToString());
    }

    private IEnumerator LoadNewAdditiveSceneAsync(SceneName newScene, bool newSceneAsActiveScene = false) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        if (newSceneAsActiveScene) {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(GetScene(newScene));
        }

    }

    private IEnumerator LoadNewGlobalAdditiveSceneAsync(SceneName newScene, bool newSceneAsActiveScene) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        SceneLoadData sld = new SceneLoadData(newScene.ToString());
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);

        if (newSceneAsActiveScene) {
            sld.PreferredActiveScene = new SceneLookupData(newScene.ToString());
        }
    }

    private IEnumerator TestingStartHost() {
        NetworkConnection conn = InstanceFinder.ClientManager.Connection;
        Debug.Log("TESTING NETWORK CONN: " + conn.ClientId);
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.PlayerScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        SceneLoadData sld = new SceneLoadData(SceneName.PlayerScene.ToString());
        //InstanceFinder.SceneManager.LoadConnectionScenes(conn, sld);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        
        asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.GameScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        sld = new SceneLoadData(SceneName.GameScene.ToString());
        //InstanceFinder.SceneManager.LoadConnectionScenes(conn, sld);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);

        sld.PreferredActiveScene = new SceneLookupData(SceneName.GameScene.ToString());
    }

    public void LoadFromMainMenuToGameScene() {
        StartCoroutine(TestingStartHost());

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }

    public void LoadFromMainMenuToGameSceneAsGuest() {
        //StartCoroutine(TestingStartHost());

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }

    public void UnloadMainMenuScene() {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
}