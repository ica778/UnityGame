using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {
    public enum Scene {
        MainMenuScene,
        GameScene,
    }

    private static Scene currentScene;

    public static void Load(Scene sceneToLoad) {
        currentScene = sceneToLoad;

        SceneManager.LoadScene(currentScene.ToString());
    }

    public static Scene GetCurrentScene() {
        return currentScene;
    }
}