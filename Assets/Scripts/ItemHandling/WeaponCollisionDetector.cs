using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollisionDetector : MonoBehaviour {
    [SerializeField] private Collider boxCollider;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("TESTING 123: " + other.gameObject.layer.ToString());
    }

    public void DisableCollider() {
        boxCollider.enabled = false;
    }

    public void EnableCollider() {
        boxCollider.enabled = true;
    }
}