using FishNet.Managing;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DefaultMenuUI : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button testHostButton;
    [SerializeField] private Button testJoinButton;

    public event EventHandler OnHostButtonClick;
    public event EventHandler OnJoinButtonClick;
    public event EventHandler OnTestHostButtonClick;
    public event EventHandler OnTestJoinButtonClick;

    private void Start() {
        hostButton.onClick.AddListener(() => {
            OnHostButtonClick?.Invoke(this, EventArgs.Empty);
        });

        joinButton.onClick.AddListener(() => {
            OnJoinButtonClick?.Invoke(this, EventArgs.Empty);
        });

        testHostButton.onClick.AddListener(() => {
            OnTestHostButtonClick?.Invoke(this, EventArgs.Empty);
        });

        testJoinButton.onClick.AddListener(() => {
            OnTestJoinButtonClick?.Invoke(this, EventArgs.Empty);
        });
    }
}