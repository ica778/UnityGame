using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

public class MainMenuManager : MonoBehaviour {
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject defaultMenuUI;
    [SerializeField] private GameObject lobbyBrowserUI;

    private void Awake() {
        Instance = this;
        ShowDefaultMenu();
    }

    private void CloseUI() {
        defaultMenuUI.SetActive(false);
        lobbyBrowserUI.SetActive(false);
    }

    public void ShowDefaultMenu() {
        CloseUI();
        defaultMenuUI.SetActive(true);
    }

    public void ShowLobbyBrowser() {
        CloseUI();
        lobbyBrowserUI.SetActive(true);
    }


}