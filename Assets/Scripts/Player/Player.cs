using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : NetworkBehaviour {

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask walkableLayer;

    private PlayerMovement playerMovement;
    private float interactDistance = 5f;
    private RaycastHit currentInteractableObject;

    private void Start() {
        PlayerManager.Instance.AddPlayer(base.ObjectId, GetComponent<Player>());
        playerMovement = GetComponentInChildren<PlayerMovement>();

        Debug.Log("CLIENT CONNECTED WITH ID: " + base.ObjectId);
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }

        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnDropAction += GameInput_OnDropAction;
    }

    

    private void Update() {
        HighlightInteractableObject();
    }

    private void GameInput_OnDropAction(object sender, System.EventArgs e) {
        DropItem();
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (currentInteractableObject.transform) {
            GroundLoot groundLoot = currentInteractableObject.transform.GetComponent<GroundLoot>();

            if (groundLoot) {
                PickupGroundLoot(groundLoot);
            }

        }
    }

    private void DropItem() {
        ItemSO item = InventoryManager.Instance.GetSelectedItem(true);
        if (item) {
            Vector3 dropItemPosition = cameraPosition.transform.position + (PlayerCameraController.LocalInstance.GetCameraTransform().forward * 1.5f);
            if (!Physics.Raycast(cameraPosition.transform.position, PlayerCameraController.LocalInstance.GetCameraTransform().forward, 1.5f, walkableLayer)) {
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, PlayerCameraController.LocalInstance.GetCameraQuaternionOnlyYAxis());
            }
            else {
                dropItemPosition = cameraPosition.transform.position;
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, PlayerCameraController.LocalInstance.GetCameraQuaternionOnlyYAxis());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(GameObject groundLootItemToDrop, Vector3 dropItemPosition, Quaternion quaternion) {
        GameObject item = Instantiate(groundLootItemToDrop, dropItemPosition, quaternion);
        Spawn(item);
    }

    private void PickupGroundLoot(GroundLoot groundLoot) {
        bool addedItemSuccessfully = InventoryManager.Instance.AddItem(groundLoot.GetItem());

        if (addedItemSuccessfully) {
            DespawnItemObjectServerRpc(currentInteractableObject.transform.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnItemObjectServerRpc(GameObject gameObjectToDespawn) {
        ServerManager.Despawn(gameObjectToDespawn, DespawnType.Destroy);
    }

    private void HighlightInteractableObject() {
        if (Physics.Raycast(cameraPosition.position, PlayerCameraController.LocalInstance.GetLookDirectionVector(), out currentInteractableObject, interactDistance, interactableLayer)) {
            //Debug.Log("Interactable Object Detected");
        }
    }

    public PlayerMovement GetPlayerMovement() {
        return playerMovement;
    }
}