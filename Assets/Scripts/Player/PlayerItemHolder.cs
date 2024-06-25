using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : MonoBehaviour {
    private ItemSO currentItem;
    private GameObject currentItemObject;

    private void InventoryManager_OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        if (currentItemObject) {
            Destroy(currentItemObject);
        }

        if (e.itemId == -1) {
            currentItem = null;
            currentItemObject = null;
        }
        else {
            currentItem = ItemDatabase.Instance.GetItem(e.itemId);
            currentItemObject = Instantiate(currentItem.GetEquippedObject());
            currentItemObject.transform.SetParent(transform, false);
        }
    }

    private void OnEnable() {
        InventoryManager.Instance.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
    }

    private void OnDisable() {
        InventoryManager.Instance.OnSelectedItemChanged -= InventoryManager_OnSelectedItemChanged;
    }
}