using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public static class MainMenuToGameSceneHandler {
    public static void LoadIntoGame(bool asHost) {
        if (asHost) {
            SceneLoadData sld = new SceneLoadData(SceneName.GameBootstrapScene.ToString());
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
}