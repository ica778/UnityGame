using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerMovement playerMovement;
    private float interactDistance = 5f;
    private RaycastHit currentInteractableObject;

    private void Start() {
        PlayerManager.Instance.AddPlayer(base.ObjectId, GetComponent<Player>());

        Debug.Log("CLIENT CONNECTED WITH ID: " + base.ObjectId);
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }

        playerMovement = GetComponentInChildren<PlayerMovement>();

        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnDropAction += GameInput_OnDropAction;
    }

    private void GameInput_OnDropAction(object sender, System.EventArgs e) {
        Debug.Log("TESTING DROP ITEM");
    }

    private void Update() {
        HighlightInteractableObject();
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

    private void PickupGroundLoot(GroundLoot groundLoot) {
        bool addedItemSuccessfully = InventoryManager.Instance.AddItem(groundLoot.GetItem());

        if (addedItemSuccessfully) {
            DespawnItemObjectServerRpc(currentInteractableObject.transform.gameObject);
        }
    }

    [ServerRpc]
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