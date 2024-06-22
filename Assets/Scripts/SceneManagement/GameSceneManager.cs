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

    private List<SceneLoadData> sldList = new();

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

        SceneLoadData sld = new SceneLoadData(SceneName.GamePersistentObjectsScene.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        sld = new SceneLoadData(SceneName.CaravanScene.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        sld = new SceneLoadData(SceneName.GameScene1.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        SceneLookupData slud = new SceneLookupData(SceneName.GameScene1.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    // NOTE: this function works with the assumption the last scene is the current level scene
    public void SwitchLevelSceneForAllClients(SceneName sceneToSwitchTo) {
        if (!base.IsServerInitialized) {
            Debug.LogError("ERROR: SwitchLevelSceneForAllClients CALLED BY CLIENT");
        }

        SceneLoading.Instance.ResetClientsLoaded();
        SceneLoading.Instance.WaitForClientsToLoadScenes(new int[] {sceneToSwitchTo.GetHashCode()});

        SceneUnloadData sud = new SceneUnloadData(sldList[sldList.Count - 1].SceneLookupDatas);
        base.SceneManager.UnloadGlobalScenes(sud);

        sldList.RemoveAt(sldList.Count - 1);

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
    }
}