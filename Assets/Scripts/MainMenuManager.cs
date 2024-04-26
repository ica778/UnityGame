using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private ConnectionManager connectionManager;

    private NetworkManager networkManager;

    private void Awake() {
        Instance = this;

        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null) {
            Debug.LogError("NetworkManager not found");
            return;
        }

        ShowMainMenuUI();
    }

    public void OnHostButtonClick() {
        /*
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
        */
        connectionManager.StartHost();
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void OnJoinButtonClick() {
        //networkManager.ClientManager.StartConnection();
        connectionManager.StartConnection();
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    private void ShowMainMenuUI() {
        CloseUI();
        mainMenuUI.SetActive(true);
    }

    private void CloseUI() {
        mainMenuUI.SetActive(false);
    }
}