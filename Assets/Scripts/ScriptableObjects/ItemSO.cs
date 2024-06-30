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
    [SerializeField] private GameObject groundLootPrefab;
    [SerializeField] private GameObject equippedPrefab;

    private int id;

    public string ItemName => itemName;

    public ItemType ItemType => itemType;

    public bool IsStackable => stackable;

    public Sprite Sprite => sprite;

    public int GetMaxStackCount() {
        if (stackable) {
            return maxStackCount;
        }
        return -1;
    }

    public GameObject GroundLootObject => groundLootPrefab;

    public GameObject EquippedObject => equippedPrefab;

    public int Id => id;

    public void SetId(int i) {
        id = i;
    }
}