using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private DefaultMenuUI defaultMenuUI;
    [SerializeField] private LobbyBrowserUI lobbyBrowserUI;

    private void Awake() {
        ShowDefaultMenu();
    }

    private void CloseUI() {
        defaultMenuUI.gameObject.SetActive(false);
        lobbyBrowserUI.gameObject.SetActive(false);
    }

    private void ShowDefaultMenu() {
        CloseUI();
        defaultMenuUI.gameObject.SetActive(true);
    }

    private void ShowLobbyBrowser() {
        CloseUI();
        lobbyBrowserUI.gameObject.SetActive(true);
    }

    private void LobbyBrowserUI_OnBackButtonClick(object sender, System.EventArgs e) {
        ShowDefaultMenu();
    }

    private void DefaultMenuUI_OnJoinButtonClick(object sender, System.EventArgs e) {
        ShowLobbyBrowser();
    }

    private void OnEnable() {
        lobbyBrowserUI.OnBackButtonClick += LobbyBrowserUI_OnBackButtonClick;
        defaultMenuUI.OnJoinButtonClick += DefaultMenuUI_OnJoinButtonClick;
    }

    private void OnDisable() {
        lobbyBrowserUI.OnBackButtonClick -= LobbyBrowserUI_OnBackButtonClick;
        defaultMenuUI.OnJoinButtonClick -= DefaultMenuUI_OnJoinButtonClick;
    }
}