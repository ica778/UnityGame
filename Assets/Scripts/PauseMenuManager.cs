using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour {
    [SerializeField] private PauseMenuUI pauseMenuUI;

    private void QuitGame() {
        GameSceneManager.Instance.QuitGame();
    }

    private void PauseMenuUI_OnQuitButtonClick(object sender, System.EventArgs e) {
        QuitGame();
    }

    private void OnEnable() {
        pauseMenuUI.OnQuitButtonClick += PauseMenuUI_OnQuitButtonClick;
    }

    private void OnDisable() {
        pauseMenuUI.OnQuitButtonClick -= PauseMenuUI_OnQuitButtonClick;
    }

}