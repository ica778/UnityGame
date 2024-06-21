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

    public override void OnStartClient() {
        Instance = this;
        
        if (base.IsServerInitialized) {
            StartGameAsHost();
        }


    }

    // NOTE: this function works with the assumption the last scene is the current level scene
    public void SwitchLevelSceneForAllClients(SceneName sceneToSwitchTo) {
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

    // TODO: make the next 2 functions run clientside instead of serverside
    // Check if all clients on server have loaded scenes
    public IEnumerator WaitForAllClientsToLoad(SceneName[] scenes) {
        while (!HaveClientsLoadedScenes(scenes)) {
            yield return new WaitForSeconds(0.5f);
        }

        OnAllClientsOnServerFinishedLoadingScenes?.Invoke(this, EventArgs.Empty);
    }

    // Check if all clients on server have loaded scenes
    private bool HaveClientsLoadedScenes(SceneName[] scenes) {
        Dictionary<UnityEngine.SceneManagement.Scene, HashSet<NetworkConnection>> sceneConnections = InstanceFinder.SceneManager.SceneConnections;

        foreach (SceneName sceneName in scenes) {
            UnityEngine.SceneManagement.Scene currentScene = SceneHelper.GetScene(sceneName);
            foreach (NetworkConnection conn in InstanceFinder.ClientManager.Clients.Values) {
                if (!sceneConnections.ContainsKey(currentScene) || !sceneConnections[currentScene].Contains(conn)) {
                    return false;
                }
            }
        }
        return true;
    }
}