using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLoot : InteractableObjectBase {
    [SerializeField] private ItemSO item;

    public int Id { get; private set; }
    public int Count { get; private set; }

    private void Awake() {
        Id = item.GetID();
        base.Type = InteractableObjectType.GroundLoot;
    }

    public ItemSO GetItem() {
        return item;
    }

    public void SetCount(int count) {
        this.Count = count;
    }
}