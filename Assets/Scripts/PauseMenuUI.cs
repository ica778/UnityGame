using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour {

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
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
}