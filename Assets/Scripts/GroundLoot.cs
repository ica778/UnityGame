using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLoot : MonoBehaviour {
    [SerializeField] private int ID;
    [SerializeField] private ItemSO item;

    public int GetID() { 
        return ID; 
    }

    public ItemSO GetItem() {
        return item;
    }
}