using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    [SerializeField] private BoxCollider[] boxColliders;

    public bool CheckIfValid() {
        DisableColliders();
        foreach (BoxCollider collider in boxColliders) {
            collider.gameObject.SetActive(true);
            Vector3 center = collider.bounds.center;
            Vector3 extents = collider.bounds.extents;
            collider.gameObject.SetActive(false);

            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, Quaternion.identity, dungeonValidatorColliderLayer);
            if (overlappingColliders.Length != 0) {
                Debug.Log("TESTING OVERLAPPING!!!");
                EnableColliders();
                return false;
            }
        }
        EnableColliders();
        return true;
    }

    public void DisableColliders() {
        foreach (BoxCollider collider in boxColliders) {
            collider.gameObject.SetActive(false);
        }
    }

    public void EnableColliders() {
        foreach (BoxCollider collider in boxColliders) {
            collider.gameObject.SetActive(true);
        }
    }
}