using FishNet.Managing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Start() {
        hostButton.onClick.AddListener(() => {
            MainMenuManager.Instance.OnHostButtonClick();
        });

        joinButton.onClick.AddListener(() => {
            MainMenuManager.Instance.OnJoinButtonClick();
        });
    }
}