using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemSO> itemList = new List<ItemSO>();
    [SerializeField] private List<GroundLoot> groundLootList = new List<GroundLoot>();

    private void Awake() {
        Instance = this;
    }

    public ItemSO GetItem(int ID) {
        return itemList[ID];
    }

    public ItemSO GetItem(GroundLoot groundLoot) {
        return itemList[groundLoot.GetID()];
    }

    public GroundLoot GetGroundLoot(int ID) {
        return groundLootList[ID];
    }

    public GroundLoot GetGroundLoot(ItemSO item) {
        return groundLootList[item.GetID()];
    }

    public Mesh GetMesh(int ID) {
        return groundLootList[ID].GetComponentInChildren<MeshFilter>().sharedMesh;
    }

    public Material GetMaterial(int ID) {
        return groundLootList[ID].GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }
}