using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;

public class GameSceneManager : NetworkBehaviour {
    public override void OnStartClient() {
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            LoadScenesForHostServerRpc(conn);
        }
        else {
            StartCoroutine(LoadingScenesForClientAsync(conn));
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e) {
        NetworkConnection conn = base.ClientManager.Connection;
        if (base.IsServerInitialized) {
            LoadNextGameSceneForHostServerRpc(conn);
            //StartCoroutine(TestingTestingTestingHostLoadScenes(conn));
            LoadNextGameScenesForClients();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadNextGameSceneForHostServerRpc(NetworkConnection conn) {
        SceneUnloadData sud = new SceneUnloadData("GameScene1");
        base.SceneManager.UnloadConnectionScenes(conn, sud);

        SceneLoadData sld = new SceneLoadData("GameScene2");
        base.SceneManager.LoadConnectionScenes(conn, sld);
        SceneLookupData slud = new SceneLookupData("GameScene2");
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    /*
    private IEnumerator TestingTestingTestingHostLoadScenes(NetworkConnection conn) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }

        asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("GameScene1");
        while (!asyncOperation.isDone) {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));

        TestingTestingTestingServerRpc(conn);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestingTestingTestingServerRpc(NetworkConnection conn) {
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));

        SceneLookupData slud = new SceneLookupData("GameScene1");
        SceneUnloadData sud = new SceneUnloadData(slud);
        base.SceneManager.UnloadConnectionScenes(conn, sud);
    }
    */

    [ServerRpc(RequireOwnership = false)]
    private void LoadScenesForHostServerRpc(NetworkConnection conn) {
        SceneLoadData sld = new SceneLoadData("GamePersistentObjectsScene");
        base.SceneManager.LoadConnectionScenes(conn, sld);

        sld = new SceneLoadData("GameScene1");
        base.SceneManager.LoadConnectionScenes(conn, sld);
        SceneLookupData slud = new SceneLookupData("GameScene1");
        sld.PreferredActiveScene = new PreferredScene(slud);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddClientToConnectionSceneServerRpc(NetworkConnection conn) {
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePersistentObjectsScene"));
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene"));
    }

    private IEnumerator LoadingScenesForClientAsync(NetworkConnection conn) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GamePersistentObjectsScene", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }

        asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene1", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene1"));

        AddClientToConnectionSceneServerRpc(conn);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddClientToNextGameSceneServerRpc(NetworkConnection conn) {
        base.SceneManager.AddConnectionToScene(conn, UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene2"));
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadNextGameScenesForClients() {
        LoadingNextGameScenesForClients();
    }

    [ObserversRpc(ExcludeServer = true)]
    private void LoadingNextGameScenesForClients() {
        NetworkConnection conn = base.ClientManager.Connection;
        StartCoroutine(LoadingNextGameScenesForClientAsync(conn));
    }

    private IEnumerator LoadingNextGameScenesForClientAsync(NetworkConnection conn) {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene2", LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            yield return null;
        }

        asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("GameScene1");
        while (!asyncOperation.isDone) {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene2"));

        AddClientToNextGameSceneServerRpc(conn);
    }
}