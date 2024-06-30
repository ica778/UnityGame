using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : NetworkBehaviour {

    public override void OnStartNetwork() {
        SceneLoading.Instance.OnFinishedLoadingStartScenes += SceneLoading_OnFinishedLoadingStartScenes;
        SceneLoading.Instance.OnFinishedLoadingLevelScenes += SceneLoading_OnFinishedLoadingLevelScenes;
        SceneLoading.Instance.OnHostFinishedLoadingStartScenes += SceneLoading_OnHostFinishedLoadingStartScenes;
    }

    public override void OnStopNetwork() {
        SceneLoading.Instance.OnFinishedLoadingStartScenes -= SceneLoading_OnFinishedLoadingStartScenes;
        SceneLoading.Instance.OnFinishedLoadingLevelScenes -= SceneLoading_OnFinishedLoadingLevelScenes;
        SceneLoading.Instance.OnHostFinishedLoadingStartScenes -= SceneLoading_OnHostFinishedLoadingStartScenes;
    }

    private void SceneLoading_OnFinishedLoadingStartScenes(object sender, System.EventArgs e) {
        Debug.Log("THIS CLIENT HAS FINISHED LOADING INITIAL SCENES");
    }

    private void SceneLoading_OnFinishedLoadingLevelScenes(object sender, System.EventArgs e) {
        Debug.Log("THIS CLIENT HAS FINISHED LOADING LEVEL SCENES");
    }

    private void SceneLoading_OnHostFinishedLoadingStartScenes(object sender, System.EventArgs e) {
        Debug.Log("HOST HAS FINISHED LOADING INITIAL SCENES");
    }
}