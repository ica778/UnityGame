using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: PUT THIS SCRIPT ON SAME GAMEOBJECT AS HITBOX COLLIDER
[RequireComponent(typeof(Collider))]
public class DamageReceiver : MonoBehaviour {

    public void ReceiveHit(float damage) {
        Debug.Log("TESTING RECEIVING HIT FOR DAMAGE: " + damage);
    }
}