using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoading : NetworkBehaviour {
    public static SceneLoading Instance { get; private set; }

    public event EventHandler OnHostFinishedLoadingStartScenes;
    public event EventHandler OnFinishedLoadingStartScenes;
    public event EventHandler OnFinishedLoadingLevelScenes;
    private Dictionary<NetworkConnection, bool> clientsLoaded;

    private int timesToCheckIfLoadingComplete = 20;

    public override void OnStartNetwork() {
        Instance = this;

        if (base.IsServerInitialized) {
            base.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientFinishedLoadingStartScenes;
        }
    }

    public override void OnStopNetwork() {
        if (base.IsServerInitialized) {
            base.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientFinishedLoadingStartScenes;
        }
    }

    /// <summary>
    /// Wait for host to finish loading start scenes.
    /// </summary>
    /// <param name="sceneNames">Integer array of SceneNames of start scenes</param>
    public void WaitForHostToLoadStartScenes(SceneName[] sceneNames) {
        StartCoroutine(WaitForHostToLoadStartScenesAsync(sceneNames));
    }

    private IEnumerator WaitForHostToLoadStartScenesAsync(SceneName[] sceneNames) {
        int timesToCheck = timesToCheckIfLoadingComplete;
        while (!CheckIfScenesAreLoaded(sceneNames) && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(0.5f);
        }

        if (timesToCheck <= 0) {
            Debug.LogError("ERROR: SCENE NOT LOADING");
        }

        OnHostFinishedLoadingStartScenes?.Invoke(this, EventArgs.Empty);
    }

    private void SceneManager_OnClientFinishedLoadingStartScenes(NetworkConnection arg1, bool arg2) {
        ClientFinishedLoadingStartScenesTargetRpc(arg1);
    }

    /// <summary>
    /// Notify client of NetworkConnection conn that they are done loading start scenes.
    /// </summary>
    [TargetRpc]
    private void ClientFinishedLoadingStartScenesTargetRpc(NetworkConnection conn) {
        OnFinishedLoadingStartScenes?.Invoke(this, EventArgs.Empty);
    }

    public void ResetClientsLoaded() {
        clientsLoaded = new();
        foreach (NetworkConnection conn in base.ClientManager.Clients.Values) {
            clientsLoaded[conn] = false;
        }
    }

    /// <summary>
    /// ObserversRpc that tells clients to wait for themselves to load scenes then notifies server when done loading.
    /// </summary>
    /// <param name="sceneNameHashCodes">Integer array of SceneName HashCodes</param>
    [ObserversRpc]
    public void WaitForClientsToLoadScenes(int[] sceneNameHashCodes) {
        SceneName[] sceneNames = new SceneName[sceneNameHashCodes.Length];

        for (int i = 0; i < sceneNameHashCodes.Length; i++) {
            sceneNames[i] = (SceneName)sceneNameHashCodes[i];
        }

        StartCoroutine(WaitForClientToLoadScenes(sceneNames));

    }

    private IEnumerator WaitForClientToLoadScenes(SceneName[] sceneNames) {
        int timesToCheck = timesToCheckIfLoadingComplete;
        while (!CheckIfScenesAreLoaded(sceneNames) && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(0.5f);
        }

        if (timesToCheck <= 0) {
            Debug.LogError("ERROR: SCENE NOT LOADING");
        }

        NotifyServerThatClientFinishedLoadingScenesServerRpc(base.LocalConnection);
    }

    private bool CheckIfScenesAreLoaded(SceneName[] sceneNames) {
        foreach (SceneName sceneName in sceneNames) {
            if (!SceneHelper.GetScene(sceneName).isLoaded) {
                return false;
            }
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerThatClientFinishedLoadingScenesServerRpc(NetworkConnection conn) {
        clientsLoaded[conn] = true;
        foreach (bool current in clientsLoaded.Values) {
            if (!current) {
                return;
            }
        }

        // server notify all clients everyone is done loading level scenes
        NotifyClientsFinishedLoadingLevelScenes();
    }

    [ObserversRpc]
    private void NotifyClientsFinishedLoadingLevelScenes() {
        OnFinishedLoadingLevelScenes?.Invoke(this, EventArgs.Empty);
    }
}