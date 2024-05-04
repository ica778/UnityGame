using UnityEngine;
using UnityEngine.UI;

public class DefaultMenuUI : MonoBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private DefaultMenuManager defaultMenuManager;

    private void Awake() {
        defaultMenuManager = GetComponent<DefaultMenuManager>();
    }

    private void Start() {
        hostButton.onClick.AddListener(() => {
            defaultMenuManager.OnClickHostButton();
        });

        joinButton.onClick.AddListener(() => {
            defaultMenuManager.OnClickJoinButton();
        });
    }
}