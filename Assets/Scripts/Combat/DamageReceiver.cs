using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: PUT THIS SCRIPT ON THE HITBOXES PARENT. SO THE GAME OBJECT THIS SCRIPT IS ON SHOULD BE PARENT OF COLLIDERS WHICH ARE HITBOXES.
public class DamageReceiver : NetworkBehaviour {

    public void ReceiveHit(int damage) {
        Debug.Log("TESTING RECEIVING HIT FOR DAMAGE: " + damage);
        ReceiveHitServerRpc(damage, base.LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReceiveHitServerRpc(int damage, NetworkConnection conn) {
        ReceiveHitObserversRpc(damage, conn);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ReceiveHitObserversRpc(int damage, NetworkConnection conn) {
        if (base.LocalConnection == conn) {
            return;
        }
        Debug.Log("TESTING RECEIVING HIT BY ANOTHER PLAYER FOR DAMAGE: " + damage);
    }
}