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

    public override void OnStartNetwork() {
        if (base.IsServerInitialized) {
            SceneLoading.Instance.OnFinishedLoadingLevelScenes += SceneLoading_OnFinishedLoadingLevelScenes;
        }
    }

    public override void OnStopNetwork() {
        if (base.IsServerInitialized) {
            SceneLoading.Instance.OnFinishedLoadingLevelScenes -= SceneLoading_OnFinishedLoadingLevelScenes;
        }
    }

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

            UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene == SceneHelper.GetScene(SceneName.GameScene1)) {
                GameSceneManager.Instance.SwitchLevelSceneForAllClients(SceneName.GameScene);
            }
            else if (activeScene == SceneHelper.GetScene(SceneName.GameScene)) {
                GameSceneManager.Instance.SwitchLevelSceneForAllClients(SceneName.GameScene1);
            }

            SceneName[] testingSceneArr = new SceneName[] { this.destination };
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

    // NOTE: this event will only fire on the server
    private void SceneLoading_OnFinishedLoadingLevelScenes(object sender, EventArgs e) {
        ResumeMovingCaravanObserversRpc();
    }
}