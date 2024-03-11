using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
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

    public void OnDrop(PointerEventData eventData) {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        inventoryItem.SetParentAfterDrag(transform);
        if (transform.childCount > 0) {
            transform.GetComponentInChildren<InventoryItem>().transform.SetParent(inventoryItem.GetParentBeforeDrag());
        }
    }
}