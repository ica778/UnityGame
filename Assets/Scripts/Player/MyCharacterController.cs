using FishNet.Object;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public enum CharacterMovementState {
    Walking,
    Sprinting,
    Crouching,
}

public class MyCharacterController : NetworkBehaviour, ICharacterController {

    [SerializeField] public KinematicCharacterMotor motor;
    [SerializeField] private Transform meshRoot;

    // ground movement
    private float currentMaxMoveSpeed = 1f;
    private Vector3 moveInputVector = Vector3.zero;
    private float moveSharpness = 15f;
    private float maxWalkingMoveSpeed = 10f;
    private float maxSprintingMoveSpeed = 20f;
    private float maxCrouchingMoveSpeed = 5f;


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

    // crouching
    public bool ShouldBeCrouching { get; private set; } = false;
    public bool IsCrouching { get; private set; } = false;

    // misc
    private Vector3 gravity = new Vector3 (0, -30f, 0);
    private Vector3 internalVelocityAdd = Vector3.zero;
    private Collider[] probedColliders = new Collider[8];

    public CharacterMovementState CurrentCharacterMovementState { get; private set; }

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            motor.enabled = false;
            this.enabled = false;
        }
    }

    private void Start() {
        motor.CharacterController = this;

        TransitionToState(CharacterMovementState.Walking);
    }

    private void TransitionToState(CharacterMovementState newState) {
        CharacterMovementState initialState = CurrentCharacterMovementState;
        OnStateExit(initialState, newState);
        CurrentCharacterMovementState = newState;
        OnStateEnter(newState, initialState);
    }

    private void OnStateEnter(CharacterMovementState state, CharacterMovementState fromState) {
        switch (state) {
            case CharacterMovementState.Walking:
                currentMaxMoveSpeed = maxWalkingMoveSpeed;
                break;
            case CharacterMovementState.Sprinting:
                currentMaxMoveSpeed = maxSprintingMoveSpeed;
                break;
            case CharacterMovementState.Crouching:
                currentMaxMoveSpeed = maxCrouchingMoveSpeed;
                break;
        }
    }

    private void OnStateExit(CharacterMovementState state, CharacterMovementState toState) {
        switch (state) {
            case CharacterMovementState.Walking:
                break;
            case CharacterMovementState.Sprinting:
                break;
            case CharacterMovementState.Crouching: 
                break;
        }
    }

    public void RequestSprint() {
        if (CurrentCharacterMovementState == CharacterMovementState.Walking) {
            TransitionToState(CharacterMovementState.Sprinting);
        }
    }

    public void RequestWalk() {
        if (CurrentCharacterMovementState == CharacterMovementState.Sprinting) {
            TransitionToState(CharacterMovementState.Walking);
        }
    }

    public void SetLookRotationInput(float lookYRotation) {
        lookRotation = Quaternion.Euler(0, lookYRotation, 0);
    }

    public void SetMovementVectorInput(Vector3 movementVectorInput) {
        moveInputVector = movementVectorInput;
    }

    public void RequestJump() {
        if (CurrentCharacterMovementState == CharacterMovementState.Crouching) {
            RequestUncrouch();
        }
        timeSinceJumpRequested = 0f;
        jumpRequested = true;
    }

    public void RequestCrouch() {
        ShouldBeCrouching = true;
        IsCrouching = true;
        motor.SetCapsuleDimensions(0.4f, 1f, 0.5f);
        meshRoot.localScale = new Vector3(1f, 0.5f, 1f);
        
        TransitionToState(CharacterMovementState.Crouching);

        CrouchServerRpc(base.ObjectId);
    }

    public void RequestUncrouch() {
        ShouldBeCrouching = false;
        if (IsCrouching && !ShouldBeCrouching) {
            // Do an overlap test with the character's standing height to see if there are any obstructions
            motor.SetCapsuleDimensions(0.4f, 1.8f, 0.9f);
            if (motor.CharacterCollisionsOverlap(
                    motor.TransientPosition,
                    motor.TransientRotation,
                    probedColliders) > 0) {
                // If obstructions, just stick to crouching dimensions
                motor.SetCapsuleDimensions(0.4f, 1f, 0.5f);
            }
            else {
                // If no obstructions, uncrouch
                meshRoot.localScale = new Vector3(1f, 1f, 1f);
                IsCrouching = false;
                TransitionToState(CharacterMovementState.Walking);

                UncrouchServerRpc(base.ObjectId);
            }
        }
    }

    // TODO: IMPLEMENT THIS
    [ServerRpc]
    private void CrouchServerRpc(int playerId) {
        CrouchObserversRpc(playerId);
    }

    // TODO: IMPLEMENT THIS
    [ObserversRpc(ExcludeOwner = true)]
    private void CrouchObserversRpc(int playerId) {
        // Crouching logic here
        Debug.Log("TESTING PLAYER ID: " + playerId + " IS CROUCHING");
    }

    // TODO: IMPLEMENT THIS
    [ServerRpc]
    private void UncrouchServerRpc(int playerId) {
        UncrouchObserversRpc(playerId);
    }

    // TODO: IMPLEMENT THIS
    [ObserversRpc(ExcludeOwner = true)]
    private void UncrouchObserversRpc(int playerId) {
        // Uncrouching logic here
        Debug.Log("TESTING PLAYER ID: " + playerId + " IS NO LONGER CROUCHING");
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
            Vector3 inputRight = Vector3.Cross(moveInputVector, motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * moveInputVector.magnitude;
            targetMovementVelocity = reorientedInput * currentMaxMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-moveSharpness * deltaTime));
        }
        else {
            // Add move input
            if (moveInputVector.sqrMagnitude > 0f) {
                targetMovementVelocity = moveInputVector * maxAirMoveSpeed;

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
            // See if we are in correct stance to jump
            if (CurrentCharacterMovementState == CharacterMovementState.Walking || CurrentCharacterMovementState == CharacterMovementState.Sprinting) {
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

        // Take into account additive velocity
        if (internalVelocityAdd.sqrMagnitude > 0f) {
            currentVelocity += internalVelocityAdd;
            internalVelocityAdd = Vector3.zero;
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

        // Handle landing and leaving ground
        if (motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround) {
            OnLanded();
        }
        else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround) {
            OnLeaveStableGround();
        }
    }

    private void OnLeaveStableGround() {
        // This is called when player leaves ground
        maxAirMoveSpeed = currentMaxMoveSpeed;
    }

    private void OnLanded() {
        // This is called when player lands on ground
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) {
        // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    }

    public void AddVelocity(Vector3 velocity) {
        // This is used to add velocity for things like knockback, explosions, windzones, etc
        internalVelocityAdd += velocity;
    }
}