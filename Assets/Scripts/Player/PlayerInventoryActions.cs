using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerInventoryActions : NetworkBehaviour {
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private LayerMask layersYouCantDropItemInto;

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            this.enabled = false;
            return;
        }
    }

    public void DropCurrentItem() {
        ItemSO item = InventoryManager.Instance.GetSelectedItem(true);
        if (item) {
            Vector3 dropItemPosition = cameraTarget.transform.position + (cameraTarget.transform.forward * 1.5f);
            if (!Physics.Raycast(cameraTarget.transform.position, cameraTarget.transform.forward, 1.5f, layersYouCantDropItemInto)) {
                DropItemServerRpc(item.GroundLootObject, dropItemPosition, cameraTarget.transform.rotation);
            }
            else {
                dropItemPosition = cameraTarget.transform.position;
                DropItemServerRpc(item.GroundLootObject, dropItemPosition, cameraTarget.transform.rotation);
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

    private void GameInput_OnDropAction(object sender, System.EventArgs e) {
        DropCurrentItem();
    }

    private void OnEnable() {
        GameInput.Instance.OnDropAction += GameInput_OnDropAction;
    }

    private void OnDisable() {
        GameInput.Instance.OnDropAction -= GameInput_OnDropAction;
    }
}