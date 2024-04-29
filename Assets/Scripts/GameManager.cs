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
        /*
        ConnectionManager.Instance.DisconnectFromServer();
        GameInput.Instance.UnlockCursor();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        */

        LobbyHandler.Instance.TestingKickUsers();
    }
}