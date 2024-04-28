using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour {
    public void QuitGame() {
        LobbyHandler.Instance.DestroySelf();
        ConnectionManager.Instance.StopConnection();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }
}