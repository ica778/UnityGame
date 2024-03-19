using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
    private enum MoveState {
        Walking,
        Sprinting,
        Crouching,
    }

    [SerializeField] private Transform groundPoint;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject standingCapsule;
    [SerializeField] private GameObject crouchingCapsule;
    private Rigidbody rigidBody;
    private CapsuleCollider standingCapsuleCollider;
    private CapsuleCollider crouchingCapsuleCollider;

    private float maxWalkSpeed = 5f;
    private float maxSprintSpeed = 10f;
    private float maxCrouchSpeed = 3f;
    private MoveState currentMoveState = MoveState.Walking;
    private bool isJumping = false;
    private float jumpTimerCooldown = 0.25f;
    private float jumpTimer;

    private float groundDrag = 15f;
    private float moveForceMultiplier = 200f;
    private float moveForceNotGroundedMultiplier = 0.1f;

    private float jumpForce = 10f;

    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;

    private bool isGrounded;
    private bool isOnWalkableAngle;
    private float maxWalkableAngle = 45f;
    // MAKE SURE THESE TWO FLOATS ARE THE SAME, MAYBE COMBINE THEM LATER ON
    private float isGroundedRaycastRange = 0.4f;
    private float walkableAngleRaycastRange = 0.4f;
    private RaycastHit slopeHit;

    private float stepRaycastDistance = 0.4f;
    private float stepHeight = 2f;
    private float stepRaycastRange = 0.6f;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        standingCapsuleCollider = standingCapsule.GetComponent<CapsuleCollider>();
        crouchingCapsuleCollider = crouchingCapsule.GetComponent<CapsuleCollider>();
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }

        GameInput.Instance.LockCursor();
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnSprintStartedAction += GameInput_OnSprintStartedAction;
        GameInput.Instance.OnSprintCancelledAction += GameInput_OnSprintCancelledAction;
        GameInput.Instance.OnCrouchAction += GameInput_OnCrouchAction;
    }

    private void Update() {
        if (!base.IsOwner) {
            return;
        }

        UpdateIsGroundedState();
        UpdateMoveDirection();
        UpdateIsOnWalkableAngleState();
        UpdateIsJumpingState();
        UpdateSlopeMoveDirection();
        UpdateRigidBodyDrag();

        // TODO: FIND A BETTER WAY TO ACTIVATE INTERPOLATION THAN THIS
        if (rigidBody.interpolation != RigidbodyInterpolation.Interpolate) {
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void FixedUpdate() {
        if (!base.IsOwner) {
            return;
        }

        ToggleGravityIfOnWalkableAngle();
        MovePlayer();
        if (moveDirection != Vector3.zero) {
            ClimbStep();
        }
        SpeedControl();
    }

    private void UpdateMoveDirection() {
        moveDirection = orientation.forward * GameInput.Instance.GetMoveVector().y + orientation.right * GameInput.Instance.GetMoveVector().x;
    }

    private void UpdateSlopeMoveDirection() {
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void UpdateRigidBodyDrag() {
        if (isOnWalkableAngle && !isJumping) {
            rigidBody.drag = groundDrag;
        }
        else {
            rigidBody.drag = 0;
        }
    }

    private void UpdateIsGroundedState() {
        isGrounded = Physics.CheckSphere(groundPoint.position, isGroundedRaycastRange, walkableLayer);
    }

    private void UpdateIsOnWalkableAngleState() {
        if (Physics.Raycast(groundPoint.position, Vector3.down, out slopeHit, walkableAngleRaycastRange, walkableLayer)) {
            float angleOfGround = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angleOfGround <= maxWalkableAngle) {
                isOnWalkableAngle = true;
                return;
            }
            else {
                isOnWalkableAngle = false;
                return;
            }
        }
        isOnWalkableAngle = false;
    }

    private void UpdateIsJumpingState() {
        if (jumpTimer <= 0 && isGrounded) {
            isJumping = false;
        }

        if (jumpTimer > 0f) {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void GameInput_OnSprintStartedAction(object sender, System.EventArgs e) {
        currentMoveState = MoveState.Sprinting;
    }

    private void GameInput_OnSprintCancelledAction(object sender, System.EventArgs e) {
        currentMoveState = MoveState.Walking;
    }

    private void GameInput_OnCrouchAction(object sender, System.EventArgs e) {
        ToggleCrouchState();
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        Jump();
    }

    private void Jump() {
        if (isOnWalkableAngle && isGrounded) {
            isJumping = true;
            isGrounded = false;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumpTimer = jumpTimerCooldown;
        }
    }

    private void ToggleGravityIfOnWalkableAngle() {
        rigidBody.useGravity = !isOnWalkableAngle;
    }

    private void MovePlayer() {
        if (isOnWalkableAngle && isGrounded) {
            rigidBody.AddForce(slopeMoveDirection.normalized * moveForceMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded) {
            rigidBody.AddForce(moveDirection.normalized * moveForceMultiplier * moveForceNotGroundedMultiplier, ForceMode.Acceleration);
        }
    }

    private void SpeedControl() {
        if (isGrounded && !isJumping) {
            if (currentMoveState == MoveState.Walking) {
                if (rigidBody.velocity.magnitude > maxWalkSpeed) {
                    rigidBody.velocity = rigidBody.velocity.normalized * maxWalkSpeed;
                }
            }
            else if (currentMoveState == MoveState.Sprinting) {
                if (rigidBody.velocity.magnitude > maxSprintSpeed) {
                    rigidBody.velocity = rigidBody.velocity.normalized * maxSprintSpeed;
                }
            }
            else if (currentMoveState == MoveState.Crouching) {
                if (rigidBody.velocity.magnitude > maxCrouchSpeed) {
                    rigidBody.velocity = rigidBody.velocity.normalized * maxCrouchSpeed;
                }
            }
        }
        else {
            Vector3 currentPlayerVelocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
            if (currentMoveState == MoveState.Walking) {
                if (currentPlayerVelocity.magnitude > maxWalkSpeed) {
                    Vector3 limitedSpeed = currentPlayerVelocity.normalized * maxWalkSpeed;
                    rigidBody.velocity = new Vector3(limitedSpeed.x, rigidBody.velocity.y, limitedSpeed.z);
                }
            }
            else if (currentMoveState == MoveState.Sprinting) {
                if (currentPlayerVelocity.magnitude > maxSprintSpeed) {
                    Vector3 limitedSpeed = currentPlayerVelocity.normalized * maxSprintSpeed;
                    rigidBody.velocity = new Vector3(limitedSpeed.x, rigidBody.velocity.y, limitedSpeed.z);
                }
            }
            else if (currentMoveState == MoveState.Crouching) {
                if (currentPlayerVelocity.magnitude > maxCrouchSpeed) {
                    Vector3 limitedSpeed = currentPlayerVelocity.normalized * maxCrouchSpeed;
                    rigidBody.velocity = new Vector3(limitedSpeed.x, rigidBody.velocity.y, limitedSpeed.z);
                }
            }
        }

    }

    private void ClimbStep() {
        Vector3 stepRaycastLowerPosition = groundPoint.transform.position;
        Vector3 stepRaycastUpperPosition = new Vector3(stepRaycastLowerPosition.x, stepRaycastLowerPosition.y + stepRaycastDistance, stepRaycastLowerPosition.z);

        if (Physics.Raycast(stepRaycastLowerPosition, moveDirection, stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveDirection, stepRaycastRange + 0.1f, walkableLayer)) {
                rigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
        else if (Physics.Raycast(stepRaycastLowerPosition, moveDirection + new Vector3(1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveDirection + new Vector3(1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                rigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
        else if (Physics.Raycast(stepRaycastLowerPosition, moveDirection + new Vector3(-1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveDirection + new Vector3(-1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                rigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
    }

    private void ToggleCrouchState() {
        ToggleRigidBodyForCrouchState();
        ToggleCrouchingServerRpc(base.ObjectId);
    }

    public void ToggleRigidBodyForCrouchState() {
        // checks for standingCapsuleCollider instead of currentMoveState because currentMoveState is not guaranteed set by this function
        if (standingCapsuleCollider.enabled) {
            standingCapsuleCollider.enabled = false;
            crouchingCapsuleCollider.enabled = true;
            cameraPosition.transform.position -= new Vector3(0f, 0.8f, 0f);
        }
        else {
            standingCapsuleCollider.enabled = true;
            crouchingCapsuleCollider.enabled = false;
            cameraPosition.transform.position += new Vector3(0f, 0.8f, 0f);
        }
    }

    [ServerRpc]
    private void ToggleCrouchingServerRpc(int playerId) {
        ToggleCrouchingObserversRpc(playerId);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ToggleCrouchingObserversRpc(int playerId) {
        Player player = PlayerManager.Instance.GetPlayer(playerId);
        player.GetPlayerMovement().ToggleRigidBodyForCrouchState();
    }

    public bool IsWalking() {
        if (currentMoveState == MoveState.Walking && moveDirection != Vector3.zero) {
            return true;
        }
        return false;
    }

    public bool IsSprinting() {
        if (currentMoveState == MoveState.Sprinting && moveDirection != Vector3.zero) {
            return true;
        }
        return false;
    }
}
