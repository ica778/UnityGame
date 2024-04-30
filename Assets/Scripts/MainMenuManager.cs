using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

public class MainMenuManager : MonoBehaviour {
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameServerBrowserManager gameServerBrowserManager;

    private void Awake() {
        Instance = this;

        ShowMainMenuUI();
    }

    private void Start() {
        gameServerBrowserManager.evtSearchCompleted.AddListener(OnServerSearchCompleted);
    }

    public void OnHostButtonClick() {
        ConnectionManager.Instance.StartHost();
        LobbyHandler.Instance.CreateLobby();
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    /*
    public void OnJoinButtonClick() {
        gameServerBrowserManager.GetAllInternet();
    }
    */

    public void OnJoinButtonClick() {
        //connectionManager.StartConnection();
        Debug.LogError("FEATURE NOT IMPLEMENTED YET");
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    private void OnServerSearchCompleted(ResultData resultData) {
        if (resultData.entries != null) {
            resultData.entries.ForEach(entry => {
                Debug.Log("TESTING " + entry.SteamId);
            });
            Debug.Log("TESTING 123: " + resultData.entries.Count);
        }
        
        
        
        
    }

    private void ShowMainMenuUI() {
        CloseUI();
        mainMenuUI.SetActive(true);
    }

    private void CloseUI() {
        mainMenuUI.SetActive(false);
    }
}