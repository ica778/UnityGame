using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




public class MyCharacterController : MonoBehaviour, ICharacterController {

    [SerializeField] private KinematicCharacterMotor motor;

    // ground movement
    private float maxMoveSpeed = 10f;
    private Vector3 moveVector = Vector3.zero;
    public float moveSharpness = 15f;

    // air movement
    private float maxAirMoveSpeed = 10f;
    private float airAccelerationSpeed = 5f;
    private float drag = 0.1f;

    // camera rotation
    private Quaternion lookRotation;

    // jumping
    private bool allowJumpingWhenSliding = false;
    private float jumpSpeed = 10f;
    private float jumpPreGroundingGraceTime = 0f;
    private float jumpPostGroundingGraceTime = 0f;

    private bool jumpRequested = false;
    private bool jumpConsumed = false;
    private bool jumpedThisFrame = false;
    private float timeSinceJumpRequested = Mathf.Infinity;
    private float timeSinceLastAbleToJump = 0f;

    // misc
    private Vector3 gravity = new Vector3 (0, -30f, 0);

    private void Start() {
        motor.CharacterController = this;
    }

    public void SetLookRotationInput(float lookYRotation) {
        lookRotation = Quaternion.Euler(0, lookYRotation, 0);
    }

    public void SetMovementVectorInput(Vector3 movementVectorInput) {
        moveVector = movementVectorInput;
    }

    public void RequestJump() {
        timeSinceJumpRequested = 0f;
        jumpRequested = true;
    }

    public void BeforeCharacterUpdate(float deltaTime) {
        // This is called before the motor does anything
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
        // This is called when the motor wants to know what its rotation should be right now
        currentRotation = lookRotation;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        // This is called when the motor wants to know what its velocity should be right now
        Vector3 targetMovementVelocity = Vector3.zero;
        if (motor.GroundingStatus.IsStableOnGround) {
            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
            currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(moveVector, motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * moveVector.magnitude;
            targetMovementVelocity = reorientedInput * maxMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-moveSharpness * deltaTime));
        }
        else {
            // Add move input
            if (moveVector.sqrMagnitude > 0f) {
                targetMovementVelocity = moveVector * maxMoveSpeed;

                // Prevent climbing on un-stable slopes with air movement
                if (motor.GroundingStatus.FoundAnyGround) {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                currentVelocity += velocityDiff * airAccelerationSpeed * deltaTime;
            }

            // Gravity
            currentVelocity += gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (drag * deltaTime)));
        }

        // Handle jumping
        jumpedThisFrame = false;
        timeSinceJumpRequested += deltaTime;
        if (jumpRequested) {
            // See if we actually are allowed to jump
            if (!jumpConsumed && ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)) {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = motor.CharacterUp;
                if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround) {
                    jumpDirection = motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                motor.ForceUnground(0.1f);

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * jumpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                jumpRequested = false;
                jumpConsumed = true;
                jumpedThisFrame = true;
            }
        }
    }

    public void AfterCharacterUpdate(float deltaTime) {
        // This is called after the motor has finished everything in its update

        // Handle jump-related values
        {
            // Handle jumping pre-ground grace period
            if (jumpRequested && timeSinceJumpRequested > jumpPreGroundingGraceTime) {
                jumpRequested = false;
            }

            // Handle jumping while sliding
            if (allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) {
                // If we're on a ground surface, reset jumping values
                if (!jumpedThisFrame) {
                    jumpConsumed = false;
                }
                timeSinceLastAbleToJump = 0f;
            }
            else {
                // Keep track of time since we were last able to jump (for grace period)
                timeSinceLastAbleToJump += deltaTime;
            }
        }
    }

    public bool IsColliderValidForCollisions(Collider coll) {
        // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
        // This is called when the motor's ground probing detects a ground hit
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
        // This is called when the motor's movement logic detects a hit
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) {
        // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
    }

    public void PostGroundingUpdate(float deltaTime) {
        // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }
}