using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using System.Collections.Generic;
using System.Linq;
using FishNet;

public class GameSceneManager : NetworkBehaviour {
    public static GameSceneManager Instance { get; private set; }

    private List<SceneLoadData> sldList = new();

    public override void OnStartClient() {
        Instance = this;
        
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            StartGameAsHostServerRpc(conn);
        }
        else {
            StartGameAsClientServerRpc(conn);
        }
    }

    public void OnCaravanLeverPulled() {
        SwitchScenesFromCaravanLeverServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwitchScenesFromCaravanLeverServerRpc() {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene == UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneName.GameScene1.ToString())) {
            LoadNewLevelScene(SceneName.GameScene.ToString());
        }
        else if (activeScene == UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneName.GameScene.ToString())) {
            LoadNewLevelScene(SceneName.GameScene1.ToString());
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
}