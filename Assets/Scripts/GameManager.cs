using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Start() {
        Instance = this;
        LobbyHandler.Instance.GetLobbyManager().evtAskedToLeave.AddListener(OnAskedToLeave);
    }

    private void OnAskedToLeave() {
        ConnectionManager.Instance.DisconnectFromServer();
    }

    public void QuitGame() {
        ConnectionManager.Instance.DisconnectFromServer();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }
}