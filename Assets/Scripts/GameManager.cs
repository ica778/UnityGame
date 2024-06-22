using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void QuitGame() {
        Debug.LogError("TODO: FEATURE NOT IMPLEMENTED YET");

        ConnectionManager.Instance.Disconnect();
        GameInput.Instance.UnlockCursor();
        LobbyHandler.Instance.DestroySelf();
        
    }
}