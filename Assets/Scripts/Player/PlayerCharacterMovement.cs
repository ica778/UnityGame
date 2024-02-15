using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCharacterMovement : MonoBehaviour {
    private enum MoveState {
        Walking,
        Sprinting,
        Crouching,
    }

    [SerializeField] private float maxWalkSpeed = 7f;
    [SerializeField] private float maxSprintSpeed = 10f;
    [SerializeField] private float maxCrouchSpeed = 3f;

    [SerializeField] Transform playerLookDirection;
    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] Transform feetRaycastLocation;

    [SerializeField] private LayerMask walkableLayer;

    [SerializeField] private float maxSlopeAngle = 50;
    [SerializeField] private float groundDrag = 10f;

    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float airMovementSpeedMultiplier = 0.25f;
    
    [SerializeField] private float crouchYScale = 0.5f;

    private PlayerInputActions playerInputActions;
    private MoveState currentMoveState;
    private Vector3 playerMoveDirectionVector;
    private Vector3 playerMoveVector;

    private bool isGrounded = true;
    private bool isOnSlope = false;
    private float currentMoveSpeedMax = 7f;
    private bool jumping = false;
    private RaycastHit slopeHit;
    private float jumpTimer = 0f;
    private float playerCapsuleStartYScale;

    [SerializeField] private float checkSlopeRaycastRange = 0.5f;

    [SerializeField] private Transform playerStepRaycast;
    [SerializeField] private GameObject stepRaycastUpper;
    [SerializeField] private GameObject stepRaycastLower;
    private float stepHeight = 0.5f;
    private float stepSmooth = 2f;
    private float stepRaycastRange = 0.5f;

    private void Start() {
        playerRigidBody = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;

        currentMoveState = MoveState.Walking;

        playerCapsuleStartYScale = transform.localScale.y;
    }

    private void Update() {
        playerMoveVector = playerInputActions.Player.Movement.ReadValue<Vector3>();

        stepRaycastUpper.transform.position = new Vector3(stepRaycastUpper.transform.position.x, stepHeight, stepRaycastUpper.transform.position.z);

        playerStepRaycast.transform.position = feetRaycastLocation.transform.position;

        CheckIfPlayerIsGrounded();
        CheckIfPlayerOnSlope();

        HandleMovementStates();
        SetPlayerRigidBodyDrag();
        HandleJumping();
    }

    private void FixedUpdate() {
        SpeedControl();
        MovePlayerCharacter();
        ClimbStep();
        Crouching();

        //Debug.Log("SPEED: " + playerRigidBody.velocity.magnitude);
        //Debug.Log(isGrounded + " | " + isOnSlope);
    }

    private void MovePlayerCharacter() {
        // get movement in camera direction independent of camera's y angle
        playerMoveDirectionVector = playerLookDirection.forward * playerMoveVector.z + playerLookDirection.right * playerMoveVector.x;
        playerMoveDirectionVector.y = 0f;
        playerMoveDirectionVector = playerMoveDirectionVector.normalized;

        if (isOnSlope) {
            playerMoveDirectionVector = Vector3.ProjectOnPlane(playerMoveDirectionVector, slopeHit.normal).normalized;

            if (playerRigidBody.velocity.y > 0f) {
                playerRigidBody.AddForce(Vector3.down * 20f, ForceMode.Force);
            }
        }

        playerRigidBody.useGravity = !isOnSlope;

        if (isGrounded) {
            playerRigidBody.AddForce(playerMoveDirectionVector * currentMoveSpeedMax * 20f, ForceMode.Force);
        }
        else {
            playerRigidBody.AddForce(playerMoveDirectionVector * currentMoveSpeedMax * airMovementSpeedMultiplier * 20f, ForceMode.Force);
        }
        
    }

    private void SpeedControl() {
        if (isOnSlope && !jumping) {
            if (playerRigidBody.velocity.magnitude > currentMoveSpeedMax) {
                playerRigidBody.velocity = playerRigidBody.velocity.normalized * currentMoveSpeedMax;
            }
        }
        else {
            Vector3 currentPlayerVelocity = new Vector3(playerRigidBody.velocity.x, 0f, playerRigidBody.velocity.z);
            if (currentPlayerVelocity.magnitude > currentMoveSpeedMax) {
                Vector3 limitedSpeed = currentPlayerVelocity.normalized * currentMoveSpeedMax;
                playerRigidBody.velocity = new Vector3(limitedSpeed.x, playerRigidBody.velocity.y, limitedSpeed.z);
            }
        }
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isGrounded && !jumping) {
            Jump();
            jumpTimer = 0.5f;
            jumping = true;
        }
    }

    private void Jump() {
        playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckIfPlayerIsGrounded() {
        isGrounded = Physics.CheckSphere(feetRaycastLocation.transform.position, 0.3f, walkableLayer);
    }

    private void CheckIfPlayerOnSlope() {
        if (Physics.Raycast(feetRaycastLocation.transform.position, Vector3.down, out slopeHit, checkSlopeRaycastRange)) {
            float angleOfGround = Vector3.Angle(Vector3.up, slopeHit.normal);
            isOnSlope = angleOfGround < maxSlopeAngle && angleOfGround != 0;
        }
        else {
            isOnSlope = false;
        }
    }

    private void HandleMovementStates() {
        Vector3 playerMoveInput = playerInputActions.Player.Movement.ReadValue<Vector3>();
        if (playerInputActions.Player.Sprint.IsPressed() && playerMoveInput.z >= 1) {
            currentMoveState = MoveState.Sprinting;
        }
        else if (playerInputActions.Player.Crouch.IsPressed()) {
            // sprinting will intentionally override crouching
            currentMoveState = MoveState.Crouching;
        }
        else {
            currentMoveState = MoveState.Walking;
        }

        switch (currentMoveState) {
            case MoveState.Walking:
                currentMoveSpeedMax = maxWalkSpeed;
                break;
            case MoveState.Sprinting:
                currentMoveSpeedMax = maxSprintSpeed;
                break;
            case MoveState.Crouching:
                currentMoveSpeedMax = maxCrouchSpeed;
                break;
        }
    }

    private void SetPlayerRigidBodyDrag() {
        if (isGrounded) {
            playerRigidBody.drag = groundDrag;
        }
        else {
            playerRigidBody.drag = 0;
        }
    }

    private void HandleJumping() {
        if (jumpTimer <= 0 && (isGrounded || isOnSlope)) {
            jumping = false;
        }

        if (jumpTimer > 0f) {
            jumpTimer -= Time.deltaTime;
        }
    }

    public Vector3 GetMovementVector() {
        return this.playerMoveDirectionVector;
    }

    private void ClimbStep() {
        Vector3 playerMoveDirection = GetMovementVector();

        if (Physics.Raycast(stepRaycastLower.transform.position, playerMoveDirection, stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpper.transform.position, playerMoveDirection, stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.position += new Vector3(0f, stepSmooth * Time.deltaTime, 0f);
            }
        }

        if (Physics.Raycast(stepRaycastLower.transform.position, playerMoveDirection + new Vector3(1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpper.transform.position, playerMoveDirection + new Vector3(1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.position += new Vector3(0f, stepSmooth * Time.deltaTime, 0f);
            }
        }

        if (Physics.Raycast(stepRaycastLower.transform.position, playerMoveDirection + new Vector3(-1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpper.transform.position, playerMoveDirection + new Vector3(-1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.position += new Vector3(0f, stepSmooth * Time.deltaTime, 0f);
            }
        }
    }

    private void Crouching() {
        if (currentMoveState == MoveState.Crouching) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
        else {
            transform.localScale = new Vector3(transform.localScale.x, playerCapsuleStartYScale, transform.localScale.z);
        }
    }

}