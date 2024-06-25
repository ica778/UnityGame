using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemSO> itemList = new List<ItemSO>();

    private void Awake() {
        Instance = this;
    }

    public ItemSO GetItem(int ID) {
        return itemList[ID];
    }

    public ItemSO GetItem(GroundLoot groundLoot) {
        return itemList[groundLoot.Id];
    }
}