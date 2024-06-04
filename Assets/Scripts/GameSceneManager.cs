using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using System.Collections.Generic;
using System.Linq;

public class GameSceneManager : NetworkBehaviour {

    private string persistentObjectScene = "GamePersistentObjectsScene";
    private string gameScene = "GameScene";
    private string gameScene1 = "GameScene1";
    private string gameScene2 = "GameScene2";

    private List<SceneLoadData> sldList = new();

    public override void OnStartClient() {
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            StartGameAsHostServerRpc(conn);
        }
        else {
            StartGameAsClientServerRpc(conn);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e) {
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            LoadNewLevelScene(gameScene);
        }
        else {
            LoadNewLevelScene(gameScene2);
        }
    }

    // NOTE: this function works with the assumption the last scene is the level scene
    [ServerRpc(RequireOwnership = false)]
    private void LoadNewLevelScene(string sceneToSwitchTo) {
        NetworkConnection[] conns = base.ServerManager.Clients.Values.ToArray();

        SceneUnloadData sud = new SceneUnloadData(sldList[sldList.Count - 1].SceneLookupDatas);
        base.SceneManager.UnloadConnectionScenes(conns, sud);

        sldList.RemoveAt(sldList.Count - 1);

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo);
        base.SceneManager.LoadConnectionScenes(conns, sld);
        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo);
        sld.PreferredActiveScene = new PreferredScene(slud);
        sldList.Add(sld);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameAsClientServerRpc(NetworkConnection conn) {
        foreach (SceneLoadData sld in sldList) {
            base.SceneManager.LoadConnectionScenes(conn, sld);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameAsHostServerRpc(NetworkConnection conn) {
        SceneLoadData sld = new SceneLoadData(persistentObjectScene);
        base.SceneManager.LoadConnectionScenes(conn, sld);
        sldList.Add(sld);

        sld = new SceneLoadData(gameScene1);
        base.SceneManager.LoadConnectionScenes(conn, sld);
        SceneLookupData slud = new SceneLookupData(gameScene1);
        sld.PreferredActiveScene = new PreferredScene(slud);
        sldList.Add(sld);
    }
}