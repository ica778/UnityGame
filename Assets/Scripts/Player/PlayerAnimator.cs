using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private const string IS_WALKING = "IsWalking";
    private const string IS_SPRINTING = "IsSprinting";

    [SerializeField] private PlayerMovement playerMovement;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        animator.SetBool(IS_WALKING, playerMovement.IsWalking());
        animator.SetBool(IS_SPRINTING, playerMovement.IsSprinting());
    }
}