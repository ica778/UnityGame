using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SceneHandler : MonoBehaviour {
    public enum SceneName {
        None,
        BootstrapScene,
        MainMenuScene,
        PlayerScene,
        GameScene,
        GameBootstrapScene,
    }

    public static SceneHandler Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private Scene GetScene(SceneName scene) {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene.ToString());
    }

    private IEnumerator LoadIntoGameAsync() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.GameBootstrapScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(GetScene(SceneName.GameBootstrapScene));

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }

    public void LoadIntoGame() {
        StartCoroutine(LoadIntoGameAsync());
    }

    /*
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

    public void LoadFromMainMenuToGameScene() {
        StartCoroutine(LoadNewGlobalAdditiveSceneAsync(SceneName.PlayerScene, false));
        StartCoroutine(LoadNewGlobalAdditiveSceneAsync(SceneName.GameScene, true));

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }

    public void UnloadMainMenuScene() {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
    */
}