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
        
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            StartGameAsHost(conn);
        }
        else {
            StartGameAsClientServerRpc(conn);
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
        NetworkConnection[] conns = base.ServerManager.Clients.Values.ToArray();

        SceneUnloadData sud = new SceneUnloadData(sldList[sldList.Count - 1].SceneLookupDatas);
        base.SceneManager.UnloadConnectionScenes(conns, sud);

        sldList.RemoveAt(sldList.Count - 1);

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo.ToString());
        base.SceneManager.LoadConnectionScenes(conns, sld);
        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
        sldList.Add(sld);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameAsClientServerRpc(NetworkConnection conn) {
        foreach (SceneLoadData sld in sldList) {
            base.SceneManager.LoadConnectionScenes(conn, sld);
        }
    }

    private void StartGameAsHost(NetworkConnection conn) {
        SceneLoadData sld = new SceneLoadData(SceneName.GamePersistentObjectsScene.ToString());
        base.SceneManager.LoadConnectionScenes(conn, sld);
        sldList.Add(sld);

        sld = new SceneLoadData(SceneName.CaravanScene.ToString());
        base.SceneManager.LoadConnectionScenes(conn, sld);
        sldList.Add(sld);

        sld = new SceneLoadData(SceneName.GameScene1.ToString());
        base.SceneManager.LoadConnectionScenes(conn, sld);
        SceneLookupData slud = new SceneLookupData(SceneName.GameScene1.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
        sldList.Add(sld);
    }

    public void LoadScenes(Scene sceneToLoad) {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

    }
}