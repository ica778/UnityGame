using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanManager : MonoBehaviour {
    [SerializeField] private Animator animator;

    // Animations
    private const string IS_MOVING = "IsMoving";

    public void LeverPulled() {
        Debug.Log("TESTING 123 LEVER PULLED CARAVAN MANAGER TESTING 123");
    }

    private void MoveCaravanToDifferentScene() {
        animator.SetBool(IS_MOVING, true);
    }
}