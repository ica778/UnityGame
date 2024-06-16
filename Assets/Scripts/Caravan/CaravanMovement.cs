using FishNet;
using FishNet.Object;
using System;
using UnityEngine;

public class CaravanMovement : NetworkBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private CaravanManager caravanManager;

    // Animations
    private const string TRIGGER_CARAVAN_MOVE = "TriggerCaravanMove";

    [ObserversRpc]
    public void StartMovingCaravanObserversRpc() {
        animator.SetTrigger(TRIGGER_CARAVAN_MOVE);
    }

    public void PauseMovingCaravan() {
        animator.speed = 0f;
        if (base.IsServerInitialized) {
            GameSceneManager.Instance.LoadCaravanLeverPulledScenes();
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
            caravanManager.OnCaravanFinishedMoving();
        }
    }

    // TODO: RIGHT NOW THIS TRIGGERS ON ANY SCENE LOAD, FIND A WAY TO MAKE IT MORE ROBUST
    private void SceneManager_OnLoadEnd(FishNet.Managing.Scened.SceneLoadEndEventArgs obj) {
        if (base.IsServerInitialized) {
            ResumeMovingCaravan();
        }
    }

    private void OnEnable() {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }

    private void OnDisable() {
        InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }
}