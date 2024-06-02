using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using FishNet;
using FishNet.Object.Synchronizing;

public class GameBootstrapManager : NetworkBehaviour {
    private List<Scene> scenesLoaded = new();

    public override void OnStartClient() {
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            base.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
            LoadScenesForHostServerRpc(conn);
        }
        else {
            StartCoroutine(LoadingScenesForClientAsync(conn));
        }
    }
    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj) {
        if (!obj.QueueData.AsServer) {
            return;
        }

        foreach (Scene scene in obj.LoadedScenes) {
            scenesLoaded.Add(scene);
            Debug.Log("TESTING ADDING SCENE: " + scene.name);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadScenesForHostServerRpc(NetworkConnection conn) {
        SceneLoadData sld = new SceneLoadData("PlayerScene");
        base.SceneManager.LoadConnectionScenes(conn, sld);

        sld = new SceneLoadData("GameScene");
        base.SceneManager.LoadConnectionScenes(conn, sld);

        SceneLookupData slud = new SceneLookupData("GameScene");
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadScenesForClientServerRpc(NetworkConnection conn) {
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("PlayerScene"));
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    private IEnumerator LoadingScenesForClientAsync(NetworkConnection conn) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }

        asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));

        LoadScenesForClientServerRpc(conn);
    }
}