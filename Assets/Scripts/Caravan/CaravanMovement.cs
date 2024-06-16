using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanMovement : NetworkBehaviour {
    [SerializeField] private Animator animator;

    // Animations
    private const string TRIGGER_CARAVAN_MOVE = "TriggerCaravanMove";

    private bool caravanMovingLock = false;
    private SceneName destination;

    public void StartMovingCaravan() {
        if (base.IsServerInitialized && !caravanMovingLock) {
            caravanMovingLock = true;

            // TODO: THESE CONDITIONS ARE FOR TESTING, FIND A WAY TO SET THE SCENES IN GAME
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene1")) {
                this.destination = SceneName.GameScene1;
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameScene")) {
                this.destination = SceneName.GameScene;
            }
            
            StartMovingCaravanObserversRpc();
        }
    }

    [ObserversRpc]
    public void StartMovingCaravanObserversRpc() {
        animator.SetTrigger(TRIGGER_CARAVAN_MOVE);
    }

    public void PauseMovingCaravan() {
        animator.speed = 0f;
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
        animator.speed = 1f;
    }

    public void OnCaravanMovementEnd() {
        if (base.IsServerInitialized) {
            caravanMovingLock = false;
            this.destination = SceneName.GameScene1;
        }
    }

    private IEnumerator WaitForAllClientsToLoad() {
        // TODO: REPLACE THIS testingSceneArr IN THE FUTURE WITH ACTUAL ARRAY OF SCENES TO CHECK
        UnityEngine.SceneManagement.Scene[] testingSceneArr = new UnityEngine.SceneManagement.Scene[] { UnityEngine.SceneManagement.SceneManager.GetSceneByName(this.destination.ToString()) };

        while (HaveConnectedClientsLoadedScenes(testingSceneArr)) {
            yield return new WaitForSeconds(0.5f);
        }

        ResumeMovingCaravan();
    }

    private bool HaveConnectedClientsLoadedScenes(UnityEngine.SceneManagement.Scene[] scenes) {
        Dictionary<UnityEngine.SceneManagement.Scene, HashSet<NetworkConnection>> sceneConnections = InstanceFinder.SceneManager.SceneConnections;

        foreach (UnityEngine.SceneManagement.Scene scene in scenes) {
            foreach (NetworkConnection conn in InstanceFinder.ClientManager.Clients.Values) {
                if (!sceneConnections.ContainsKey(scene) || !sceneConnections[scene].Contains(conn)) {
                    return false;
                }
            }
        }
        return true;
    }
}