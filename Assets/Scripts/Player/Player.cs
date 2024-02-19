using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SerializeField] private Rigidbody playerRigidBody;
    [SerializeField] private GameObject rigidBodyCapsule;
    //private float playerCameraHeight = 1.65f;
    //private float playerCapsuleHeight;

    private CapsuleCollider capsuleCollider;

    private void Start() {
        //capsuleCollider = rigidBodyCapsule.GetComponent<CapsuleCollider>();
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }
        capsuleCollider = rigidBodyCapsule.GetComponent<CapsuleCollider>();
    }

    private void Update() {
        if (!base.IsOwner)
        {
            return;
        }
        KeepPlayerAlignedToRigidBody();
    }

    private void KeepPlayerAlignedToRigidBody() {
        transform.position = new Vector3(playerRigidBody.position.x, playerRigidBody.position.y - (capsuleCollider.height / 2f), playerRigidBody.position.z);
    }
}