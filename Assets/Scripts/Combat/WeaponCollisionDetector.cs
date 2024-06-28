using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponCollisionDetector : MonoBehaviour {
    [SerializeField] private Collider weaponCollider;

    private WeaponItemSO weaponItemSO;

    private void Awake() {
        DisableCollider();
    }

    public void SetWeaponItemSO(WeaponItemSO weaponItemSO) {
        this.weaponItemSO = weaponItemSO;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("TESTING 123: " + other.gameObject.layer.ToString());
        DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
        if (damageReceiver) {
            damageReceiver.ReceiveHit(weaponItemSO.Damage);
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