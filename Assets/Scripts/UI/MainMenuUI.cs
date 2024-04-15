using FishNet.Managing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private NetworkManager networkManager;

    private void Start() {
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null) {
            Debug.LogError("NetworkManager not found");
            return;
        }

        hostButton.onClick.AddListener(() => {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });

        joinButton.onClick.AddListener(() => {
            networkManager.ClientManager.StartConnection();
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });
    }
}