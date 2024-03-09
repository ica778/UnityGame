using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler {
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor, notSelectedColor;

    private void Awake() {
        
    }

    public void Select() {
        image.color = selectedColor;
    }

    public void Deselect() {
        image.color = notSelectedColor;
    }


    // TODO: MAKE IT SWAP ITEMS IF ITEM DROPPED ON TOP OF OCCUPIED INVENTORY SLOT
    public void OnDrop(PointerEventData eventData) {
        if (transform.childCount == 0) {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.SetParentAfterDrag(transform);
        }
    }
}