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

    public event EventHandler OnFinishedLoadingScenes;

    private List<SceneLoadData> sldList = new();

    public override void OnStartClient() {
        Instance = this;
        
        if (base.IsServerInitialized) {
            StartGameAsHost();
        }
    }

    // TODO: FIND A WAY TO SET SCENE IN GAME
    public void LoadCaravanLeverPulledScenes() {
        SwitchScenesFromCaravanLever();
    }

    private void SwitchScenesFromCaravanLever() {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene == SceneHelper.GetScene(SceneName.GameScene1)) {
            SwitchSceneForAllClients(SceneName.GameScene);
        }
        else if (activeScene == SceneHelper.GetScene(SceneName.GameScene)) {
            SwitchSceneForAllClients(SceneName.GameScene1);
        }
    }

    // NOTE: this function works with the assumption the last scene is the level scene
    private void SwitchSceneForAllClients(SceneName sceneToSwitchTo) {
        SceneUnloadData sud = new SceneUnloadData(sldList[sldList.Count - 1].SceneLookupDatas);
        base.SceneManager.UnloadGlobalScenes(sud);

        sldList.RemoveAt(sldList.Count - 1);

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    private void StartGameAsHost() {
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

    public void LoadScenes(Scene sceneToLoad) {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

    }
}