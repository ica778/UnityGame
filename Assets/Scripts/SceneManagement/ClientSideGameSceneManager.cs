using FishNet.Managing.Scened;
using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSideGameSceneManager : MonoBehaviour {
    public static ClientSideGameSceneManager Instance { get; private set; }

    public Stack<string> scenes = new();

    private void Awake() {
        Instance = this;
    }

    public void LoadIntoGame(bool asHost) {
        if (asHost) {
            SceneLoadData sld = new SceneLoadData(SceneName.GameBootstrapScene.ToString());
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }

    public void QuitGameBackToMainMenu() {
        StartCoroutine(QuitGameBackToMainMenuAsync());
    }

    private IEnumerator QuitGameBackToMainMenuAsync() {
        yield return StartCoroutine(UnloadAllGameScenesAsync());
        yield return StartCoroutine(LoadMainMenuSceneAsync());
        scenes.Clear();
    }

    private IEnumerator UnloadAllGameScenesAsync() {
        AsyncOperation asyncOperation;
        while (scenes.Count > 0) {
            UnityEngine.SceneManagement.Scene currentScene = SceneHelper.GetScene(scenes.Pop());

            asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
            while (!asyncOperation.isDone) {
                yield return null;
            }
        }
    }

    private IEnumerator LoadMainMenuSceneAsync() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.MainMenuScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(SceneHelper.GetScene(SceneName.MainMenuScene));
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if (GameSceneManager.Instance) {
            scenes.Push(arg0.name);
        }
    }

    private void SceneManager_sceneUnloaded(Scene arg0) {
        if (GameSceneManager.Instance) {
            if (scenes.Peek() == arg0.name) {
                scenes.Pop();
            }
        }
    }

    private void OnEnable() {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }



    private void OnDisable() {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }
}