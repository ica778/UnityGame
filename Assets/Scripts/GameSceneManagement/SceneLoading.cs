using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoading : NetworkBehaviour {
    public static SceneLoading Instance { get; private set; }

    public event EventHandler OnFinishedLoadingInitialGameScenes;
    public event EventHandler OnFinishedLoadingLevelScenes;
    private Dictionary<NetworkConnection, bool> clientsLoaded;

    public override void OnStartNetwork() {
        Instance = this;
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
        int timesToCheck = 20;
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

        NotifyClientsFinishedLoadingLevelScenes();
    }

    [ObserversRpc]
    private void NotifyClientsFinishedLoadingLevelScenes() {
        OnFinishedLoadingLevelScenes?.Invoke(this, EventArgs.Empty);
    }
}