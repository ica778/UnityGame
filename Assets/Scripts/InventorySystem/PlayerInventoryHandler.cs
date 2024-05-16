using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerInventoryHandler : NetworkBehaviour {
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private LayerMask layersYouCantDropItemInto;

    public void DropCurrentItem() {
        ItemSO item = InventoryManager.Instance.GetSelectedItem(true);
        if (item) {
            Vector3 dropItemPosition = playerLook.transform.position + (playerLook.transform.forward * 1.5f);
            if (!Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, 1.5f, layersYouCantDropItemInto)) {
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, playerLook.GetCameraQuaternionOnlyYAxis());
            }
            else {
                dropItemPosition = playerLook.transform.position;
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, playerLook.GetCameraQuaternionOnlyYAxis());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(GameObject groundLootItemToDrop, Vector3 dropItemPosition, Quaternion quaternion) {
        GameObject item = Instantiate(groundLootItemToDrop, dropItemPosition, quaternion);
        ServerManager.Spawn(item);
    }

    // TODO: fix potential duplication bug if more than one player pick up same object at same time. potential fix is make pickup item server authoritative.
    // This solution will introduce client side lag, but can also do client and server authoritative, and when the server authoritative part runs it can override
    // the client side. Basically this will mean if you try to dupe item by having more than one player pick up item at same time, it will temporarily show the items
    // to other players, but will then remove item when server authoritative side runs. Might also want to make changes to drop item so players cant pickup and then
    // immediately drop before server authoritative part runs to get past our anti item dupe solution.
    public void PickupGroundLoot(GroundLoot groundLoot) {
        bool addedItemSuccessfully = InventoryManager.Instance.AddItem(groundLoot.GetItem());

        if (addedItemSuccessfully) {
            groundLoot.gameObject.SetActive(false);
            DespawnItemObjectServerRpc(groundLoot.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnItemObjectServerRpc(GameObject gameObjectToDespawn) {
        ServerManager.Despawn(gameObjectToDespawn, DespawnType.Destroy);
    }
}