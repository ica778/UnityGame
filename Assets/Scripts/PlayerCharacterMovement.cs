using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterMovement : MonoBehaviour {

    private enum MoveState {
        Walking,
        Sprinting,
        Crouching,
    }

    private MoveState currentMoveState;

    [SerializeField] private float maxWalkSpeed = 7f;
    [SerializeField] private float maxSprintSpeed = 10f;
    private float maxCrouchSpeed = 3f;
    [SerializeField] Transform moveDirectionOrientation;
    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] Transform playerMiddlePoint;

    private PlayerInputActions playerInputActions;

    private float playerHeight = 2f;
    [SerializeField] private LayerMask walkableLayer;
    private bool isGrounded = true;
    private float groundDrag = 10f;
    private float currentMoveSpeedMax = 7f;

    private float jumpForce = 16f;
    private bool jumping = false;

    private float maxSlopeAngle = 50;
    private RaycastHit slopeHit;
    private float jumpTimer = 0f;

    private void Awake() {
        playerRigidBody = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;

        currentMoveState = MoveState.Walking;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (isGrounded && !jumping) {
            Jump();
            jumpTimer = 0.5f;
            jumping = true;
        }
    }

    private void Update() {
        isGrounded = Physics.Raycast(playerMiddlePoint.transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, walkableLayer);

        SetPlayerRigidBodyDrag();

        HandleJumping();

        SpeedControl();

        Debug.Log("SPEED: " + playerRigidBody.velocity.magnitude);
        //Debug.Log(jumpTimer);
    }

    private void FixedUpdate() {
        HandleMovementStates();

        MovePlayerCharacter();
    }

    private void MovePlayerCharacter() {
        Vector3 playerMoveInput = playerInputActions.Player.Movement.ReadValue<Vector3>();

        // get movement in camera direction independent of camera's y angle
        Vector3 playerMoveDirection = moveDirectionOrientation.forward * playerMoveInput.z + moveDirectionOrientation.right * playerMoveInput.x;
        playerMoveDirection.y = 0f;
        playerMoveDirection = playerMoveDirection.normalized;

        if (CheckIfPlayerOnSlope()) {
            playerMoveDirection = Vector3.ProjectOnPlane(playerMoveDirection, slopeHit.normal).normalized;

            if (playerRigidBody.velocity.y > 0f) {
                playerRigidBody.AddForce(Vector3.down * 20f, ForceMode.Force);
            }
        }

        playerRigidBody.useGravity = !CheckIfPlayerOnSlope();

        if (isGrounded) {
            playerRigidBody.AddForce(playerMoveDirection * currentMoveSpeedMax * 20f, ForceMode.Force);
        }
        
    }

    private void SpeedControl() {
        if (isGrounded) {
            if (CheckIfPlayerOnSlope() && !jumping) {
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
    }

    private void Jump() {
        playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private bool CheckIfPlayerOnSlope() {
        if (Physics.Raycast(playerMiddlePoint.transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angleOfGround = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angleOfGround < maxSlopeAngle && angleOfGround != 0;
        }
        return false;
    }

    private void HandleMovementStates() {
        if (playerInputActions.Player.Sprint.IsPressed()) {
            currentMoveState = MoveState.Sprinting;
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
        if (jumpTimer <= 0 && (isGrounded || CheckIfPlayerOnSlope())) {
            jumping = false;
        }

        if (jumpTimer > 0f) {
            jumpTimer -= Time.deltaTime;
        }
    }

}