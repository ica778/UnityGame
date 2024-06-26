using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;

public class PlayerItemHolder : NetworkBehaviour {
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
        // NOTE: THIS IS A PLACEHOLDER, I AM NOT WORKING ON VISUALS YET
        EquipItemServerRpc(base.LocalConnection.ClientId, e.itemId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EquipItemServerRpc(int playerId, int itemId) {
        EquipItemObserversRpc(playerId, itemId);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void EquipItemObserversRpc(int playerId, int itemId) {
        
    }

    private void OnEnable() {
        InventoryManager.Instance.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
    }

    private void OnDisable() {
        InventoryManager.Instance.OnSelectedItemChanged -= InventoryManager_OnSelectedItemChanged;
    }
}