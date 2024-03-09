using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSpawnItem : MonoBehaviour {
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ItemSO[] itemsToPickup;

    public void PickupItem(int ID) {
        bool result = inventoryManager.AddItem(itemsToPickup[ID]);
        if (result) {
            Debug.Log("Item added");
        }
        else {
            Debug.Log("Inventory full");
        }
    }

    public void GetSelectedItem() {
        ItemSO receivedItem = inventoryManager.GetSelectedItem(false);
        if (receivedItem) {
            Debug.Log("TESTING RECEIVED ITEM: " + receivedItem);
        }
        else {
            Debug.Log("TESTING NO ITEM RECEIVED");
        }
    }

    public void UseSelectedItem() {
        ItemSO receivedItem = inventoryManager.GetSelectedItem(true);
        if (receivedItem) {
            Debug.Log("TESTING USED ITEM: " + receivedItem);
        }
        else {
            Debug.Log("TESTING NO ITEM TO USE");
        }
    }
}
