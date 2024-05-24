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
        BootStrapScene,
        MainMenuScene,
        PlayerScene,
        GameScene,
    }

    public static SceneHandler Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        StartCoroutine(LoadNewAdditiveSceneAsync(SceneName.MainMenuScene, true));
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

    private IEnumerator LoadNewGlobalAdditiveSceneAsync(SceneName newScene, bool newSceneAsActiveScene = false) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        SceneLoadData sld = new SceneLoadData(SceneName.GameScene.ToString());
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);

        if (newSceneAsActiveScene) {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(GetScene(newScene));
        }
    }

    public void LoadFromMainMenuToGameScene() {
        StartCoroutine(LoadNewGlobalAdditiveSceneAsync(SceneName.PlayerScene, true));
        StartCoroutine(LoadNewGlobalAdditiveSceneAsync(SceneName.GameScene, true));

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
}