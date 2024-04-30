using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

public class MainMenuManager : MonoBehaviour {
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private LobbySearcher lobbySearcher;

    private void Awake() {
        Instance = this;

        ShowMainMenuUI();
    }

    public void OnHostButtonClick() {
        LobbyHandler.Instance.CreateLobby();
        ConnectionManager.Instance.StartHost();
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void OnJoinButtonClick() {
        lobbySearcher.FindLobbies();
    }

    private void ShowMainMenuUI() {
        CloseUI();
        mainMenuUI.SetActive(true);
    }

    private void CloseUI() {
        mainMenuUI.SetActive(false);
    }
}