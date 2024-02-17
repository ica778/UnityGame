using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private Rigidbody playerRigidBody;
    [SerializeField] private GameObject rigidBodyCapsule;
    private float playerCameraHeight = 1.65f;
    private float playerCapsuleHeight;

    private MeshRenderer capsuleMeshRenderer;

    private void Start() {
        capsuleMeshRenderer = rigidBodyCapsule.GetComponent<MeshRenderer>();
    }

    private void Update() {
        KeepPlayerObjectsAligned();
    }

    private void KeepPlayerObjectsAligned() {
        transform.position = new Vector3(playerRigidBody.position.x, playerRigidBody.position.y - capsuleMeshRenderer.bounds.extents.y, playerRigidBody.position.z);
    }
}