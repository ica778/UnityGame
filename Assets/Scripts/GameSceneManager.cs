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

    public event EventHandler OnClientFinishedLoadingScenes;
    public event EventHandler OnAllClientsOnServerFinishedLoadingScenes;

    private List<SceneLoadData> sldList = new();
    private Dictionary<NetworkConnection, bool> clientsLoaded;

    public override void OnStartClient() {
        Instance = this;
        
        if (base.IsServerInitialized) {
            StartGameAsHost();
        }
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

    // NOTE: this function works with the assumption the last scene is the current level scene
    public void SwitchLevelSceneForAllClients(SceneName sceneToSwitchTo) {
        if (!base.IsServerInitialized) {
            Debug.LogError("ERROR: SwitchLevelSceneForAllClients CALLED BY CLIENT");
        }

        clientsLoaded = new();
        foreach (NetworkConnection conn in base.ClientManager.Clients.Values) {
            clientsLoaded[conn] = false;
        }

        WaitForClientsToLoadScenes(new int[] {sceneToSwitchTo.GetHashCode()});

        SceneUnloadData sud = new SceneUnloadData(sldList[sldList.Count - 1].SceneLookupDatas);
        base.SceneManager.UnloadGlobalScenes(sud);

        sldList.RemoveAt(sldList.Count - 1);

        SceneLoadData sld = new SceneLoadData(sceneToSwitchTo.ToString());
        base.SceneManager.LoadGlobalScenes(sld);
        sldList.Add(sld);

        SceneLookupData slud = new SceneLookupData(sceneToSwitchTo.ToString());
        sld.PreferredActiveScene = new PreferredScene(slud);
    }


    /// <summary>
    /// ObserversRpc that tells clients to wait for themselves to load scenes then notifies server when done loading.
    /// </summary>
    /// <param name="sceneNameHashCodes">Integer array of SceneName HashCodes</param>
    [ObserversRpc]
    private void WaitForClientsToLoadScenes(int[] sceneNameHashCodes) {
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

        NotifyServerFinishedLoadingScenesServerRpc(base.LocalConnection);
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
    private void NotifyServerFinishedLoadingScenesServerRpc(NetworkConnection conn) {
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
        OnAllClientsOnServerFinishedLoadingScenes?.Invoke(this, EventArgs.Empty);
    }
}