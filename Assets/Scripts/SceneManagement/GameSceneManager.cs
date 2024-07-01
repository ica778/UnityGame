using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using System;

public class GameSceneManager : NetworkBehaviour {
    public static GameSceneManager Instance { get; private set; }

    private Stack<SceneLoadData> gameSceneSLDList = new();

    private void Awake() {
        Instance = this;
    }

    public override void OnStartClient() {
        if (base.IsServerInitialized) {
            StartGameAsHost();
        }
    }

    private void StartGameAsHost() {
        SceneName[] startingSceneNames = new SceneName[] { SceneName.GamePersistentObjectsScene, SceneName.CaravanScene, SceneName.GameScene1 };
        SceneLoading.Instance.WaitForHostToLoadStartScenes(startingSceneNames);

        for (int i = 0; i < startingSceneNames.Length; i++) {
            SceneLoadData sld = new SceneLoadData(startingSceneNames[i].ToString());
            base.SceneManager.LoadGlobalScenes(sld);
            gameSceneSLDList.Push(sld);

            if (i == startingSceneNames.Length - 1) {
                SceneLookupData slud = new SceneLookupData(SceneName.GameScene1.ToString());
                sld.PreferredActiveScene = new PreferredScene(slud);
            }
        }
    }

    // NOTE: this function works with the assumption the last scene is the current level scene
    public void SwitchLevelSceneForAllClients(SceneName sceneToSwitchTo) {
        if (!base.IsServerInitialized) {
            Debug.LogError("ERROR: SwitchLevelSceneForAllClients CALLED BY CLIENT");
        }

        SceneLoading.Instance.ResetClientsLoaded();
        SceneLoading.Instance.WaitForClientsToLoadScenes(new int[] {sceneToSwitchTo.GetHashCode()});

        SceneUnloadData sud = new SceneUnloadData(gameSceneSLDList.Peek().SceneLookupDatas);
        base.SceneManager.UnloadGlobalScenes(sud);

        gameSceneSLDList.Pop();

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        gameSceneSLDList.Push(sld);

        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    public void QuitGame() {
        StartCoroutine(QuitGameAsync());
    }

    // TODO: MAKE MORE RELIABLE
    private IEnumerator QuitGameAsync() {
        LobbyHandler.Instance.Leave();
        yield return StartCoroutine(UnloadAllGameScenesClientSideAsync());
        BootstrapManager.Instance.LoadMainMenuSceneFromGameScene();
    }

    private IEnumerator UnloadAllGameScenesClientSideAsync() {
        AsyncOperation asyncOperation;
        while (gameSceneSLDList.Count > 0) {
            UnityEngine.SceneManagement.Scene currentScene = gameSceneSLDList.Pop().GetFirstLookupScene();

            asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
            while (!asyncOperation.isDone) {
                yield return null;
            }
        }
    }

}