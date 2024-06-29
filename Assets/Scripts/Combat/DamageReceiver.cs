using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: PUT THIS SCRIPT ON THE HITBOXES PARENT. SO THE GAME OBJECT THIS SCRIPT IS ON SHOULD BE PARENT OF COLLIDERS WHICH ARE HITBOXES.
public class DamageReceiver : MonoBehaviour {

    public void ReceiveHit(float damage) {
        Debug.Log("TESTING RECEIVING HIT FOR DAMAGE: " + damage);
    }
}