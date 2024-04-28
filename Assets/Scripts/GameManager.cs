using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Start() {
        Instance = this;
    }

    public void DisconnectFromLobby() {
        LobbyHandler.Instance.DestroySelf();
        ConnectionManager.Instance.StopConnection();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }
}