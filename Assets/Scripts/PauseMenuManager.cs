using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour {
    public void QuitGame() {
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }
}