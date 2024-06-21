using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaravanMovement : NetworkBehaviour {
    [SerializeField] private Animator animator;

    // Animations
    private const string TRIGGER_CARAVAN_MOVE = "TriggerCaravanMove";

    private bool caravanMovingLock = false;
    private bool nextScenesLoadedLock = false;

    // NOTE: THIS VARIABLE NOT IN USE YET, PLANNED TO USE THIS VARIABLE WHEN SELECTING CARAVAN DESTINATIONS IN GAME
    private SceneName destination;

    public void StartMovingCaravan() {
        if (base.IsServerInitialized && !caravanMovingLock) {
            caravanMovingLock = true;

            // TODO: THESE CONDITIONS ARE FOR TESTING, FIND A WAY TO SET THE SCENES IN GAME
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene1")) {
                this.destination = SceneName.GameScene;
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene")) {
                this.destination = SceneName.GameScene1;
            }
            
            StartMovingCaravanObserversRpc();
        }
    }

    [ObserversRpc]
    public void StartMovingCaravanObserversRpc() {
        animator.SetTrigger(TRIGGER_CARAVAN_MOVE);
    }

    public void PauseMovingCaravan() {
        if (!nextScenesLoadedLock) {
            animator.speed = 0f;
        }
       
        if (base.IsServerInitialized) {
            // TODO: REPLACE SCENE LOADING FUNCTION WITH A FUNCTION THAT CAN LOAD SCENES SELECTED IN GAME
            GameSceneManager.Instance.LoadCaravanLeverPulledScenes();
            StartCoroutine(WaitForAllClientsToLoad());
        }
    }

    private void ResumeMovingCaravan() {
        if (base.IsServerInitialized) {
            ResumeMovingCaravanObserversRpc();
        }
    }

    [ObserversRpc]
    private void ResumeMovingCaravanObserversRpc() {
        nextScenesLoadedLock = true;
        animator.speed = 1f;
    }

    public void OnCaravanMovementEnd() {
        nextScenesLoadedLock = false;
        if (base.IsServerInitialized) {
            caravanMovingLock = false;

            // Set caravan destination back to hub scene
            this.destination = SceneName.GameScene1;
        }
    }

    private IEnumerator WaitForAllClientsToLoad() {
        // TODO: REPLACE THIS testingSceneArr IN THE FUTURE WITH ACTUAL ARRAY OF SCENES TO CHECK
        SceneName[] testingSceneArr = new SceneName[] { this.destination };

        while (!HaveConnectedClientsLoadedScenes(testingSceneArr)) {
            yield return new WaitForSeconds(0.5f);
        }

        ResumeMovingCaravan();
    }

    private bool HaveConnectedClientsLoadedScenes(SceneName[] sceneNames) {
        Dictionary<UnityEngine.SceneManagement.Scene, HashSet<NetworkConnection>> sceneConnections = InstanceFinder.SceneManager.SceneConnections;

        foreach (SceneName sceneName in sceneNames) {
            UnityEngine.SceneManagement.Scene currentScene = SceneHelper.GetScene(sceneName);
            foreach (NetworkConnection conn in InstanceFinder.ClientManager.Clients.Values) {
                if (!sceneConnections.ContainsKey(currentScene) || !sceneConnections[currentScene].Contains(conn)) {
                    Debug.Log("TESTING SCENES NOT LOADED FOR EVERYONE YET");
                    return false;
                }
            }
        }
        return true;
    }

    private void OnEnable() {

    }
}