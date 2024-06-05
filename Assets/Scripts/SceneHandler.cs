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

    public void LoadIntoGame(bool asHost) {
        if (asHost) {
            SceneLoadData sld = new SceneLoadData("GameBootstrapScene");
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
}