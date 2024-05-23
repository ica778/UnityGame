using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Start() {
        Instance = this;
    }

    public void QuitGame() {
        ConnectionManager.Instance.Disconnect();
        GameInput.Instance.UnlockCursor();
        LobbyHandler.Instance.DestroySelf();
        SceneHandler.Load(SceneHandler.Scenes.MainMenuScene);
    }
}