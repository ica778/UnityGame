using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private Image image;

    private Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("BEGIN DRAG");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log("DURING DRAG");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("END DRAG");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
    
    public Transform GetParentAfterDrag() {
        return parentAfterDrag;
    }

    public void SetParentAfterDrag(Transform parentAfterDrag) {
        this.parentAfterDrag = parentAfterDrag;
    }
}