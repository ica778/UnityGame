using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponCollisionDetector : MonoBehaviour {
    [SerializeField] private Collider weaponCollider;

    private void Awake() {
        DisableCollider();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("TESTING 123: " + other.gameObject.layer.ToString());
        DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
        if (damageReceiver) {
            damageReceiver.ReceiveHit(10);
        }
        
    }

    public void DisableCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = false;
        }
    }

    public void EnableCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = true;
        }
    }
}