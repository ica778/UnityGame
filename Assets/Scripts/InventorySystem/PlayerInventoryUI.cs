using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour {
    [SerializeField] private GameObject backpackInventory;

    private void Start() {
        GameInput.Instance.OnInventoryAction += GameInput_OnInventoryAction;
        backpackInventory.SetActive(false);
    }

    private void GameInput_OnInventoryAction(object sender, System.EventArgs e) {
        if (backpackInventory.activeInHierarchy) {
            Hide();
            GameInput.Instance.LockCursor();
        }
        else {
            Show();
            GameInput.Instance.UnlockCursor();
        }
    }

    private void Hide() {
        backpackInventory.SetActive(false);
    }

    private void Show() {
        backpackInventory.SetActive(true);
    }

    private void OnDestroy() {
        GameInput.Instance.OnInventoryAction -= GameInput_OnInventoryAction;
    }
}