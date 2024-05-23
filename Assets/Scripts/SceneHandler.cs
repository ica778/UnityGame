using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public static class SceneHandler {
    public enum Scenes {
        MainMenuScene,
        GameScene,
        NetworkingScene,
    }

    public static void Load(Scenes sceneToLoad) {
        SceneManager.LoadSceneAsync(sceneToLoad.ToString());
    }
}