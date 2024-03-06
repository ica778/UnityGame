using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler {

    // TODO: MAKE IT SWAP ITEMS IF ITEM DROPPED ON TOP OF OCCUPIED INVENTORY SLOT
    public void OnDrop(PointerEventData eventData) {
        if (transform.childCount == 0) {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.SetParentAfterDrag(transform);
        }
    }
}