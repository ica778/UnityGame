using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;

    private ItemSO item;
    private Transform parentAfterDrag;
    private int count = 1;

    public void InitialiseItem(ItemSO newItem) {
        item = newItem;
        image.sprite = newItem.GetSprite();
        UpdateCount();
    }

    public void UpdateCount() {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
    
    public Transform GetParentAfterDrag() {
        return parentAfterDrag;
    }

    public void SetParentAfterDrag(Transform parentAfterDrag) {
        this.parentAfterDrag = parentAfterDrag;
    }

    public ItemSO GetItem() { 
        return item; 
    }

    public int GetCount() {
        return count;
    }

    public void SetCount(int count) {
        this.count = count;
    }

    public bool IsStackable() {
        return item.IsStackable();
    }
}