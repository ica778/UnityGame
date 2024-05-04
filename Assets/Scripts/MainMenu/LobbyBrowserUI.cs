using UnityEngine;
using UnityEngine.UI;

public class LobbyBrowserUI : MonoBehaviour {
    [SerializeField] private Button backButton;

    private LobbyBrowserManager lobbyBrowserManager;

    private void Awake() {
        lobbyBrowserManager = GetComponent<LobbyBrowserManager>();
    }

    private void Start() {
        backButton.onClick.AddListener(() => {
            lobbyBrowserManager.OnClickBackButton();
        });
    }
}