using Cinemachine;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraHolder : NetworkBehaviour {
    [SerializeField] private Transform playerCharacterCameraPosition;

    private void Update() {
        transform.position = playerCharacterCameraPosition.position;
    }
}