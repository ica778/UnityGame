using FishNet.Managing.Scened;
using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using UnityEditor.Rendering;

public class GameBootstrapManager : NetworkBehaviour {
    private List<Scene> ScenesLoaded = new();

    public override void OnStartNetwork() {
        base.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }

    public override void OnStartClient() {
        NetworkConnection conn = base.ClientManager.Connection;
        LoadScenesServerRpc(conn);
    }

    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj) {
        if (!obj.QueueData.AsServer) {
            return;
        }

        foreach (Scene scene in obj.LoadedScenes) {
            ScenesLoaded.Add(scene);
        }

        Debug.Log("TESTING SCENES LOADED SIZE: " + ScenesLoaded.Count);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadScenesServerRpc(NetworkConnection conn) {
        SceneLoadData sld = new SceneLoadData("PlayerScene");
        base.SceneManager.LoadConnectionScenes(conn, sld);

        sld = new SceneLoadData("GameScene");
        base.SceneManager.LoadConnectionScenes(conn, sld);

        SceneLookupData slud = new SceneLookupData("GameScene");

        sld.PreferredActiveScene = new PreferredScene(slud);
    }
}