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

    public void LoadIntoGame(bool asHost) {
        if (asHost) {
            SceneLoadData sld = new SceneLoadData("GameBootstrapScene");
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneName.MainMenuScene.ToString());
    }
}