using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLoot : MonoBehaviour {
    [SerializeField] private int ID;
    [SerializeField] private ItemSO item;

    private int count;

    public ItemSO GetItem() {
        return item;
    }

    public void SetCount(int count) {
        this.count = count;
    }

    public int GetCount() {
        return count;
    }

    public int GetID() {
        return ID;
    }
}