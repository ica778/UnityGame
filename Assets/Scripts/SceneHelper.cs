using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneName {
    None,
    BootstrapScene,
    MainMenuScene,
    GameBootstrapScene,
    GamePersistentObjectsScene,
    GameScene,
    GameScene1,
    GameScene2,
    CaravanScene,
}

public static class SceneHelper {
    public static UnityEngine.SceneManagement.Scene GetScene(SceneName sceneName) {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName.ToString());
    }

    public static UnityEngine.SceneManagement.Scene GetScene(string sceneName) {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
    }
}