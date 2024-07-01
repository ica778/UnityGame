using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBrowserUI : MonoBehaviour {
    [SerializeField] private Button backButton;

    public event EventHandler OnBackButtonClick;

    private void Start() {
        backButton.onClick.AddListener(() => {
            OnBackButtonClick?.Invoke(this, EventArgs.Empty);
        });
    }
}