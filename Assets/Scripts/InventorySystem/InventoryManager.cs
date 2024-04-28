using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager Instance { get; private set; }

    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private int inventoryToolbarSize = 6;

    private int selectedSlotIndex;

    public event EventHandler<OnSelectedItemChangedEventArgs> OnSelectedItemChanged;
    public class OnSelectedItemChangedEventArgs : EventArgs { 
        public int idOfItemMesh;
    }

    private void Awake() {
        Instance = this;
        ChangeSelectedSlotIndex(0);
    }

    private void Start() {
        GameInput.Instance.OnScrollUpAction += GameInput_OnScrollUpAction;
        GameInput.Instance.OnScrollDownAction += GameInput_OnScrollDownAction;
        GameInput.Instance.OnLeftClickPressedAction += GameInput_OnLeftClickPressedAction;
    }

    private void Update() {

    }

    private void GameInput_OnLeftClickPressedAction(object sender, System.EventArgs e) {
        ItemSO equippedItem = GetSelectedItem(false);
        if (equippedItem) {
            Debug.Log("TESTING: USING ITEM " + equippedItem.GetItemName());
        }
    }

    private void GameInput_OnScrollUpAction(object sender, System.EventArgs e) {
        IncrementSelectedSlotIndex();
    }

    private void GameInput_OnScrollDownAction(object sender, System.EventArgs e) {
        DecrementSelectedSlotIndex();
    }

    private void ChangeSelectedSlotIndex(int slotIndex) {
        if (selectedSlotIndex >= 0) {
            inventorySlots[selectedSlotIndex].Deselect();
        }

        inventorySlots[slotIndex].Select();
        selectedSlotIndex = slotIndex;
        if (inventorySlots[selectedSlotIndex].GetComponentInChildren<InventoryItem>()) {
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                //meshFilter = GetSelectedItem(false).GetGroundLootPrefab().GetComponentInChildren<MeshFilter>()
                idOfItemMesh = GetSelectedItem(false).GetID()
            });
        }
        else {
            OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                //meshFilter = null
                idOfItemMesh = -1
            });
        }
    }

    private void IncrementSelectedSlotIndex() {
        if (selectedSlotIndex >= inventoryToolbarSize - 1) {
            ChangeSelectedSlotIndex(0);
        }
        else {
            ChangeSelectedSlotIndex(selectedSlotIndex + 1);
        }
    }

    private void DecrementSelectedSlotIndex() {
        if (selectedSlotIndex <= 0) {
            ChangeSelectedSlotIndex(inventoryToolbarSize - 1);
        }
        else {
            ChangeSelectedSlotIndex(selectedSlotIndex - 1);
        }
    }

    public bool AddItem(ItemSO item) {
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot currentInventorySlot = inventorySlots[i];
            InventoryItem currentInventoryItem = currentInventorySlot.GetComponentInChildren<InventoryItem>();
            if (
                currentInventoryItem && 
                currentInventoryItem.GetItem() == item && 
                currentInventoryItem.IsStackable() && 
                currentInventoryItem.GetCount() < currentInventoryItem.GetMaxStackCount()
             ){
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
                if (i == selectedSlotIndex) {
                    OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                        //meshFilter = item.GetGroundLootPrefab().GetComponentInChildren<MeshFilter>()
                        idOfItemMesh = item.GetID()
                    });
                }
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

    public ItemSO GetSelectedItem(bool decrementItemCount) {
        InventorySlot inventorySlot = inventorySlots[selectedSlotIndex];
        InventoryItem inventoryItemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        if (inventoryItemInSlot) {
            ItemSO item = inventoryItemInSlot.GetItem();
            if (decrementItemCount) {
                inventoryItemInSlot.SetCount(inventoryItemInSlot.GetCount() - 1);
                if (inventoryItemInSlot.GetCount() <= 0) {
                    Destroy(inventoryItemInSlot.gameObject);
                    OnSelectedItemChanged?.Invoke(this, new OnSelectedItemChangedEventArgs {
                        //meshFilter = null
                        idOfItemMesh = -1
                    });
                }
                else {
                    inventoryItemInSlot.UpdateCount();
                }
            }
            return item;
        }
        return null;
    }

    private void OnDestroy() {
        GameInput.Instance.OnScrollUpAction -= GameInput_OnScrollUpAction;
        GameInput.Instance.OnScrollDownAction -= GameInput_OnScrollDownAction;
        GameInput.Instance.OnLeftClickPressedAction -= GameInput_OnLeftClickPressedAction;
    }
}