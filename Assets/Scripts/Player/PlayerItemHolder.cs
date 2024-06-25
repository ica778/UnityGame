using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : MonoBehaviour {

    private void InventoryManager_OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        Debug.Log("TESTING ITEM CHANGED: " + e.itemId);
    }

    private void OnEnable() {
        InventoryManager.Instance.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
    }

    private void OnDisable() {
        InventoryManager.Instance.OnSelectedItemChanged -= InventoryManager_OnSelectedItemChanged;
    }
}