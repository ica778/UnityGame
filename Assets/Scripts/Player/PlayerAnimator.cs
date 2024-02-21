using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour {

    private const string IS_WALKING = "IsWalking";
    private const string IS_SPRINTING = "IsSprinting";

    [SerializeField] private PlayerMovement playerMovement;

    private Animator animator;

    private void Awake() {
        
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }

        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (!base.IsOwner) {
            return;
        }

        animator.SetBool(IS_WALKING, playerMovement.IsWalking());
        animator.SetBool(IS_SPRINTING, playerMovement.IsSprinting());
    }
}