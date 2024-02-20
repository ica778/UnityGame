using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour {
    [SerializeField] PlayerCameraController playerCameraController;

    private void Update() {
        if (!base.IsOwner) {
            return;
        }
        Quaternion rotation = playerCameraController.transform.rotation;
        //rotation.SetEulerRotation 
        transform.rotation = rotation;
    }
}