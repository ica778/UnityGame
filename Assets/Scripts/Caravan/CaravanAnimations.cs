using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanAnimations : NetworkBehaviour {
    [SerializeField] private Animator animator;

    // Animations
    private const string TRIGGER_CARAVAN_MOVE = "TriggerCaravanMove";

    private void Awake() {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }

    private void SceneManager_OnLoadEnd(FishNet.Managing.Scened.SceneLoadEndEventArgs obj) {
        if (base.IsServerInitialized) {
            ResumeAnimationServerRpc();
        }
        
        // NOTE: probably need to implement this commented-out code to verify scenes fully loaded if you choose to load more than 1 scene per level
        /*
        UnityEngine.SceneManagement.Scene gameScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneName.GameScene.ToString());
        if (animator.speed == 0f) {
            foreach (UnityEngine.SceneManagement.Scene scene in obj.LoadedScenes) {
                if (scene == gameScene) {
                    ResumeAnimation();
                }
            }
        }
        */
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResumeAnimationServerRpc() {
        ResumeAnimationObserversRpc();
    }

    [ObserversRpc]
    private void ResumeAnimationObserversRpc() {
        ResumeAnimation();
    }

    public void MoveCaravan() {
        MoveCaravanServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveCaravanServerRpc() {
        MoveCaravanObserversRpc();
    }

    [ObserversRpc]
    private void MoveCaravanObserversRpc() {
        animator.SetTrigger(TRIGGER_CARAVAN_MOVE);
    }

    public void PauseAnimation() {
        animator.speed = 0f;
        if (base.IsHostInitialized) {
            Debug.Log("TESTING CHANGED SCENE");
            GameSceneManager.Instance.LoadCaravanLeverPulledScenes();
        }
        
    }

    public void ResumeAnimation() {
        animator.speed = 1f;
    }
}