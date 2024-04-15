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
        SceneLoader.currentScene = sceneToLoad;

        SceneManager.LoadScene(SceneLoader.currentScene.ToString());
    }
}