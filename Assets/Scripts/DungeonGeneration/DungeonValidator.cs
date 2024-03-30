using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    private BoxCollider[] boxColliders;

    private void Awake() {
        boxColliders = GetComponentsInChildren<BoxCollider>();
    }

    public bool CheckIfCollidersOverlapWithOtherColliders() {
        DisableColliders();
        foreach (BoxCollider collider in boxColliders) {
            // NOTE: the 5.5f is the distance from the middle of the new room object which is 5f and 0.5f which is distance from middle of current doorway to outside
            if (Physics.CheckBox(collider.bounds.center + (collider.transform.forward * 5.51f), collider.size, collider.transform.rotation, dungeonValidatorColliderLayer)) {
                EnableColliders();
                return true;
            }
        }
        EnableColliders();
        return false;
        /*
        DisableColliders();
        foreach (BoxCollider collider in boxColliders) {
            // NOTE: the 5.5f is the distance from the middle of the new room object which is 5f and 0.5f which is distance from middle of current doorway to outside
            if (Physics.CheckBox(collider.bounds.center + (collider.transform.forward * 5.51f), collider.size, collider.transform.rotation, dungeonValidatorColliderLayer)) {
                EnableColliders();
                return true;
            }
        }
        EnableColliders();
        return false;
        */
    }

    public void DisableColliders() {
        foreach (Collider collider in boxColliders) {
            collider.gameObject.SetActive(false);
        }
    }

    public void EnableColliders() {
        foreach (Collider collider in boxColliders) {
            collider.gameObject.SetActive(true);
        }
    }
}