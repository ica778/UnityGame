using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase Instance { get; private set; }
    [SerializeField] private List<ItemSO> items = new List<ItemSO>();

    private void Awake() {
        Instance = this;
    }

    public ItemSO GetItem(int ID) {
        return items[ID];
    }
}