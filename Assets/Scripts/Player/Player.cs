using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : NetworkBehaviour {

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private Transform equippedItemHolder;
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
        GameInput.Instance.OnLeftClickPressedAction += GameInput_OnLeftClickPressedAction;
        InventoryManager.Instance.OnSelectedItemChanged += InventoryManager_OnSelectedItemChanged;
    }



    private void Update() {
        HighlightInteractableObject();
    }

    // FOR TESTING
    private void GameInput_OnLeftClickPressedAction(object sender, System.EventArgs e) {
        
    }

    private void InventoryManager_OnSelectedItemChanged(object sender, InventoryManager.OnSelectedItemChangedEventArgs e) {
        ShowEquippedItem(e);        
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
            Vector3 dropItemPosition = PlayerLook.Instance.GetCameraTransform().position + (PlayerLook.Instance.GetLookDirectionVector() * 1.5f);
            if (!Physics.Raycast(PlayerLook.Instance.GetCameraTransform().position, PlayerLook.Instance.GetLookDirectionVector(), 1.5f, walkableLayer)) {
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, PlayerLook.Instance.GetCameraQuaternionOnlyYAxis());
            }
            else {
                dropItemPosition = PlayerLook.Instance.GetCameraTransform().position;
                DropItemServerRpc(item.GetGroundLootPrefab(), dropItemPosition, PlayerLook.Instance.GetCameraQuaternionOnlyYAxis());
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
        if (Physics.Raycast(PlayerLook.Instance.GetCameraTransform().position, PlayerLook.Instance.GetLookDirectionVector(), out currentInteractableObject, interactDistance, interactableLayer)) {
            //Debug.Log("Interactable Object Detected");
        }
    }
    
    public PlayerMovement GetPlayerMovement() {
        return playerMovement;
    }

    private void ShowEquippedItem(InventoryManager.OnSelectedItemChangedEventArgs e) {
        if (!e.meshFilter) {
            equippedItemHolder.GetComponent<MeshFilter>().sharedMesh = null;
            return;
        }

        // THIS IS UNITY'S OFFICIAL WAY OF CHANGING MESH
        Mesh mesh = e.meshFilter.sharedMesh;
        Mesh mesh2 = Instantiate(mesh);
        equippedItemHolder.GetComponent<MeshFilter>().sharedMesh = mesh2;
    }
}