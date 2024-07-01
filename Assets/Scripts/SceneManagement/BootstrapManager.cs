using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour {
    public static BootstrapManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
        LoadMainMenuScene();
    }

    public void LoadMainMenuScene() {
        StartCoroutine(LoadMainMenuSceneAsync());
    }

    private IEnumerator LoadMainMenuSceneAsync() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.MainMenuScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(SceneHelper.GetScene(SceneName.MainMenuScene));
    }

    private IEnumerator UnloadGameBootstrapScene() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.GameBootstrapScene.ToString());

        while (!asyncOperation.isDone) {
            yield return null;
        }
    }

    public void LoadMainMenuSceneFromGameScene() {
        StartCoroutine(LoadMainMenuSceneFromGameSceneAsync());
    }

    private IEnumerator LoadMainMenuSceneFromGameSceneAsync() {
        yield return StartCoroutine(LoadMainMenuSceneAsync());
        yield return StartCoroutine(UnloadGameBootstrapScene());

        ConnectionManager.Instance.Disconnect();
        GameInput.Instance.UnlockCursor();
    }

}