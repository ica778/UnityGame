using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject defaultMenuUI;
    [SerializeField] private GameObject lobbyBrowserUI;

    private void Awake() {
        Instance = this;
        ShowDefaultMenu();

        StartCoroutine(StartNetworkingScene());
    }

    // NOTE: maybe duplicate code
    private IEnumerator StartNetworkingScene() {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("NetworkingScene", LoadSceneMode.Additive);

        while (!asyncOperation.isDone) {
            yield return null;
        }
    }

    private void CloseUI() {
        defaultMenuUI.SetActive(false);
        lobbyBrowserUI.SetActive(false);
    }

    public void ShowDefaultMenu() {
        CloseUI();
        defaultMenuUI.SetActive(true);
    }

    public void ShowLobbyBrowser() {
        CloseUI();
        lobbyBrowserUI.SetActive(true);
    }


}