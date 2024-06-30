using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponCollisionDetector : MonoBehaviour {
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private WeaponItemSO weaponItemSO;

    private HashSet<DamageReceiver> alreadyHitDamageReceivers = new();

    private void OnValidate() {
        if (!weaponItemSO) {
            Debug.LogError("weaponItemSO is not assigned", this);
        }
    }

    private void Awake() {
        DisableCollider();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("TESTING 123: " + other.gameObject.layer.ToString());
        DamageReceiver damageReceiver = other.GetComponentInParent<DamageReceiver>();
        if (damageReceiver && !alreadyHitDamageReceivers.Contains(damageReceiver)) {
            damageReceiver.ReceiveHit(weaponItemSO.Damage);
            alreadyHitDamageReceivers.Add(damageReceiver);
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

    public void ClearAlreadyHitDamageReceivers() {
        alreadyHitDamageReceivers.Clear();
    }
}