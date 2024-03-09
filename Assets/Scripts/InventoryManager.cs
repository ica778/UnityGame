using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager Instance { get; private set; }

    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private int maxStackCount = 5;

    private int selectedSlotIndex;

    private void Awake() {
        Instance = this;
        ChangeSelectedSlot(0);
    }

    // TODO: REPLACE INVENTORY SLOT SELECTION METHOD WITH A MORE PERMANENT SOLUTION
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeSelectedSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeSelectedSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeSelectedSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            ChangeSelectedSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            ChangeSelectedSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            ChangeSelectedSlot(5);
        }
    }

    private void ChangeSelectedSlot(int slotIndex) {
        if (selectedSlotIndex >= 0) {
            inventorySlots[selectedSlotIndex].Deselect();
        }

        inventorySlots[slotIndex].Select();
        selectedSlotIndex = slotIndex;
    }

    public bool AddItem(int ID) {
        return AddItem(ItemDatabase.Instance.GetItem(ID));
    }

    public bool AddItem(ItemSO item) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (currentInventoryItem && currentInventoryItem.GetItem() == item && currentInventoryItem.IsStackable() && currentInventoryItem.GetCount() < maxStackCount) {
                currentInventoryItem.SetCount(currentInventoryItem.GetCount() + 1);
                currentInventoryItem.UpdateCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (!currentInventoryItem) {
                SpawnNewItem(item, currentInventorySlot);
                return true;
            }
        }
        return false;
    }

    private void SpawnNewItem(ItemSO item, InventorySlot inventorySlot) {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventorySlot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    // TODO: CHECK IF DESTROY IS RIGHT FOR YOUR GAME DESIGN
    public ItemSO GetSelectedItem(bool use) {
        InventorySlot inventorySlot = inventorySlots[selectedSlotIndex];
        InventoryItem inventoryItemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        if (inventoryItemInSlot) {
            ItemSO item = inventoryItemInSlot.GetItem();
            if (use) {
                inventoryItemInSlot.SetCount(inventoryItemInSlot.GetCount() - 1);
                if (inventoryItemInSlot.GetCount() <= 0) {
                    Destroy(inventoryItemInSlot.gameObject);
                }
                else {
                    inventoryItemInSlot.UpdateCount();
                }
            }
            return inventoryItemInSlot.GetItem();
        }
        return null;
    }
}