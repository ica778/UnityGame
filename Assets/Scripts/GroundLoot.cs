using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLoot : MonoBehaviour {
    [SerializeField] private ItemSO item;

    public ItemSO GetItem() {
        return item;
    }
}