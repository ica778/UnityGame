using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour {
    [SerializeField] private Button quitButton;
    [SerializeField] private Button testButton;
    [SerializeField] private FriendInviteDropDown friendInviteDropDown;
    [SerializeField] private PauseMenuManager pauseMenuManager;

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;

        quitButton.onClick.AddListener(() => { 
            pauseMenuManager.QuitGame();
        });

        testButton.onClick.AddListener(() => {
            pauseMenuManager.QuitGame();
        });

        friendInviteDropDown.Invited.AddListener((UserData userData) => {
            LobbyHandler.Instance.InvitePlayer(userData);
        });

        Hide();
    }

    private void GameInput_OnPauseAction(object sender, System.EventArgs e) {
        if (gameObject.activeInHierarchy) {
            Hide();
            GameInput.Instance.LockCursor();
        }
        else {
            Show();
            GameInput.Instance.UnlockCursor();
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void OnDestroy() {
        GameInput.Instance.OnPauseAction -= GameInput_OnPauseAction;
    }
}