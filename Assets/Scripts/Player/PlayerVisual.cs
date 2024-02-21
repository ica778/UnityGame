using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour {

    private void Update() {
        if (!base.IsOwner) {
            return;
        }

        transform.rotation = PlayerCameraController.LocalInstance.GetCameraQuaternionOnlyYAxis();
    }
}