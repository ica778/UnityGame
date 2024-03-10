using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Loot,
    Weapon,
    Armour,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Create New Item")]
public class ItemSO : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] ItemType itemType;
    [SerializeField] private bool stackable = false;
    [SerializeField] private int maxStackCount;

    public string GetItemName() {
        return itemName;
    }

    public ItemType GetItemType() {
        return itemType;
    }

    public bool IsStackable() {
        return stackable;
    }

    public Sprite GetSprite() {
        return sprite;
    }

    public int GetMaxStackCount() {
        if (stackable) {
            return maxStackCount;
        }
        return -1;
    }
}