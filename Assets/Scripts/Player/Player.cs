using FishNet.Demo.AdditiveScenes;
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

    // TODO: fix potential duplication bug if more than one player pick up same object at same time. potential fix is make pickup item server authoritative.
    // This solution will introduce client side lag, but can also do client and server authoritative, and when the server authoritative part runs it can override
    // the client side. Basically this will mean if you try to dupe item by having more than one player pick up item at same time, it will temporarily show the items
    // to other players, but will then remove item when server authoritative side runs. Might also want to make changes to drop item so players cant pickup and then
    // immediately drop before server authoritative part runs to get past our anti item dupe solution.
    private void PickupGroundLoot(GroundLoot groundLoot) {
        bool addedItemSuccessfully = InventoryManager.Instance.AddItem(groundLoot.GetItem());

        if (addedItemSuccessfully) {
            groundLoot.gameObject.SetActive(false);
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
        if (e.idOfItemMesh == -1) {
            equippedItemHolder.GetComponent<MeshFilter>().mesh = null;
            equippedItemHolder.GetComponent<MeshRenderer>().material = null;
            ShowEquippedItemServerRpc(base.ObjectId, -1);
            return;
        }

        // THIS IS UNITY'S OFFICIAL WAY OF CHANGING MESH
        Mesh mesh = ItemDatabase.Instance.GetMesh(e.idOfItemMesh);
        equippedItemHolder.GetComponent<MeshFilter>().mesh = mesh;

        Material material = ItemDatabase.Instance.GetMaterial(e.idOfItemMesh);
        equippedItemHolder.GetComponent<MeshRenderer>().material = material;

        ShowEquippedItemServerRpc(base.ObjectId, e.idOfItemMesh);
    }

    public void SetEquippedItemMesh(int idOfItemToShow) {
        if (idOfItemToShow == -1) {
            equippedItemHolder.GetComponent<MeshFilter>().mesh = null;
            equippedItemHolder.GetComponent<MeshRenderer>().material = null;
        }
        else {
            equippedItemHolder.GetComponent<MeshFilter>().mesh = ItemDatabase.Instance.GetMesh(idOfItemToShow);
            equippedItemHolder.GetComponent<MeshRenderer>().material = ItemDatabase.Instance.GetMaterial(idOfItemToShow);
        }
    }

    
    [ServerRpc]
    private void ShowEquippedItemServerRpc(int playerId, int idOfItemToShow) {
        ShowEquippedItemObserversRpc(playerId, idOfItemToShow);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ShowEquippedItemObserversRpc(int playerId, int idOfItemToShow) {
        Player player = PlayerManager.Instance.GetPlayer(playerId);
        player.SetEquippedItemMesh(idOfItemToShow);
    }
}