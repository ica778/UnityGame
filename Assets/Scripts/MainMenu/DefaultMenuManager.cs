using FishNet;
using FishNet.Managing;
using System.Collections;
using UnityEngine;

public class DefaultMenuManager : MonoBehaviour {
    [SerializeField] private DefaultMenuUI defaultMenuUI;

    private void DefaultMenuUI_OnHostButtonClick(object sender, System.EventArgs e) {
        LobbyHandler.Instance.CreateLobby();
    }

    private void DefaultMenuUI_OnJoinButtonClick(object sender, System.EventArgs e) {
        MainMenuManager.Instance.ShowLobbyBrowser();
    }

    // NOTE: this is for testing multiplayer without steam
    private void DefaultMenuUI_OnTestHostButtonClick(object sender, System.EventArgs e) {
        ConnectionManager.Instance.StartGameAsHostOffline();
    }

    // NOTE: this is for testing multiplayer without steam
    private void DefaultMenuUI_OnTestJoinButtonClick(object sender, System.EventArgs e) {
        ConnectionManager.Instance.StartGameAsClientOffline();
        //StartCoroutine(TestingStartGameAfterDelay());
    }

    private IEnumerator TestingStartGameAfterDelay() {
        yield return new WaitForSeconds(5);
        ConnectionManager.Instance.StartGameAsClientOffline();
    }

    private void OnEnable() {
        defaultMenuUI.OnHostButtonClick += DefaultMenuUI_OnHostButtonClick;
        defaultMenuUI.OnJoinButtonClick += DefaultMenuUI_OnJoinButtonClick;
        defaultMenuUI.OnTestHostButtonClick += DefaultMenuUI_OnTestHostButtonClick;
        defaultMenuUI.OnTestJoinButtonClick += DefaultMenuUI_OnTestJoinButtonClick;
    }

    private void OnDisable() {
        defaultMenuUI.OnHostButtonClick -= DefaultMenuUI_OnHostButtonClick;
        defaultMenuUI.OnJoinButtonClick -= DefaultMenuUI_OnJoinButtonClick;
        defaultMenuUI.OnTestHostButtonClick -= DefaultMenuUI_OnTestHostButtonClick;
        defaultMenuUI.OnTestJoinButtonClick -= DefaultMenuUI_OnTestJoinButtonClick;
    }

    private void OnValidate() {
        if (!defaultMenuUI) {
            Debug.LogError("defaultMenuUI not assigned", this);
        }
    }
}