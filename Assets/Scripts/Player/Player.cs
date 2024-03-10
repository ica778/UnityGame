using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : NetworkBehaviour {

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerMovement playerMovement;
    private float interactDistance = 5f;
    private RaycastHit currentInteractableObject;

    [SerializeField] private float dropItemDistance = 1.3f;

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
            Debug.Log("TESTING INTERACTABLE ITEM FOUND");
            GroundLoot groundLoot = currentInteractableObject.transform.GetComponent<GroundLoot>();

            if (groundLoot) {
                PickupGroundLoot(groundLoot);
            }

        }
    }

    // TODO: MAKE DROPPING MORE RELIABLE IF OBSTACLE IN FRONT OF PLAYER
    private void DropItem() {
        ItemSO item = InventoryManager.Instance.GetSelectedItem(true);
        if (item) {
            Vector3 dropItemPosition = cameraPosition.transform.position + PlayerCameraController.LocalInstance.GetCameraTransform().forward;
            
            DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(GameObject groundLootItemToDrop, Vector3 dropItemPosition) {
        GameObject item = Instantiate(groundLootItemToDrop, dropItemPosition, Quaternion.identity);
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