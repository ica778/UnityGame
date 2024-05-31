using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrapManager : NetworkBehaviour {
    public override void OnStartNetwork() {
        Debug.Log("TESTING GAME BOOTSTRAP MANAGER STARTED");
    }
}