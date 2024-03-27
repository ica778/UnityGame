using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    private Collider[] colliders;

    private void Awake() {
        colliders = GetComponentsInChildren<Collider>();
    }

    public bool CheckIfCollidersOverlapWithOtherColliders() {
        DisableColliders();
        foreach (Collider collider in colliders) {
            if (Physics.CheckBox(collider.bounds.center, collider.bounds.size, Quaternion.identity, dungeonValidatorColliderLayer)) {
                EnableColliders();
                return true;
            }
        }
        EnableColliders();
        return false;
    }

    public void DisableColliders() {
        foreach (Collider collider in colliders) {
            collider.gameObject.SetActive(false);
        }
    }

    public void EnableColliders() {
        foreach (Collider collider in colliders) {
            collider.gameObject.SetActive(true);
        }
    }
}