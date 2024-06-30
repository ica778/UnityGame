using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemSO> items = new List<ItemSO>();

    private void Awake() {
        Instance = this;

        for (int i = 0; i < items.Count; i++) {
            items[i].SetId(i);
        }
    }

    public ItemSO GetItem(int ID) {
        return items[ID];
    }

    public ItemSO GetItem(GroundLoot groundLoot) {
        return items[groundLoot.Id];
    }
}