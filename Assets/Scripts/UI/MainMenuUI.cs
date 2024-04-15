using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Awake() {
        hostButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });
    }
}