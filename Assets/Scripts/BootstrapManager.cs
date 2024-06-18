using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour {
    private void Awake() {
        StartCoroutine(LoadMainMenuScene());
    }

    private IEnumerator LoadMainMenuScene() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName.MainMenuScene.ToString(), LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(SceneHelper.GetScene(SceneName.MainMenuScene));
    }
}