using Cinemachine;
using FishNet.Object;
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
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Transform cameraPosition;
    private Rigidbody playerRigidBody;

    [SerializeField] private GameObject rigidBodyStandingCapsule;
    [SerializeField] private GameObject rigidBodyCrouchingCapsule;
    //private float rigidBodyCapsuleColliderStandingHeight = 2.0f;
    //private float rigidBodyCapsuleColliderCrouchingHeight = 1.2f;

    private float maxWalkSpeed = 2f;
    private float maxSprintSpeed = 4f;
    private float maxCrouchSpeed = 0.75f;
    private float moveForce = 200f;
    private float notGroundedMoveSpeedMultiplier = 0.1f;
    private float groundDrag = 10f;
    private float currentMaxMoveSpeed = 7f;
    private Vector3 moveVector;
    private MoveState currentMoveState;

    private float maxSlopeAngle = 45f;
    private float checkSlopeRaycastRange = 0.5f;
    private RaycastHit slopeHit;

    private Quaternion playerHeading;

    private bool isGrounded = true;
    private bool isOnSlope = false;

    private float jumpForce = 16f;
    private bool jumping = false;
    private float jumpTimer;
    private float jumpTimerCooldown = 0.5f;

    private float stepRaycastDistance = 0.4f;
    private float stepHeight = 2f;
    private float stepRaycastRange = 0.6f;

    private void Awake() {

    }

    private void Start() {

    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            return;
        }

        playerRigidBody = GetComponent<Rigidbody>();

        currentMaxMoveSpeed = maxWalkSpeed;
        currentMoveState = MoveState.Walking;

        GameInput.Instance.LockCursor();
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnCrouchAction += GameInput_OnCrouchAction;
        GameInput.Instance.OnSprintStartedAction += GameInput_OnSprintStartedAction;
        GameInput.Instance.OnSprintCancelledAction += GameInput_OnSprintCancelledAction;

        PlayerCameraController.LocalInstance.SetCameraTarget(cameraPosition);
    }

    private void Update() {
        if (!base.IsOwner) {
            return;
        }
        CheckIfPlayerIsGrounded();
        CheckIfPlayerIsOnSlope();
        HandleJumpingCooldown();
    }

    private void FixedUpdate() {
        if (!base.IsOwner) {
            return;
        }
        HandleGroundedRigidBodyDrag();
        HandlePlayerMovement();
        SpeedControl();

        Debug.Log(isGrounded + " | " + isOnSlope + " | " + playerRigidBody.velocity.magnitude);
    }

    private void SetMoveState(MoveState moveState) {
        switch (moveState) {
            case MoveState.Walking:
                if (currentMoveState == MoveState.Crouching) {
                    ChangeToStandingStance();
                }
                currentMoveState = MoveState.Walking;
                currentMaxMoveSpeed = maxWalkSpeed;
                break;
            case MoveState.Sprinting:
                if (currentMoveState == MoveState.Crouching) {
                    ChangeToStandingStance();
                }
                currentMoveState = MoveState.Sprinting;
                currentMaxMoveSpeed = maxSprintSpeed;
                break;
            case MoveState.Crouching:
                ChangeToCrouchStance();
                currentMoveState = MoveState.Crouching;
                currentMaxMoveSpeed = maxCrouchSpeed;
                break;
        }
    }

    // TODO: MAKE CROUCHING SYNC ACROSS PLAYERS
    private void ChangeToCrouchStance() {
        rigidBodyStandingCapsule.gameObject.SetActive(false);
        rigidBodyCrouchingCapsule.gameObject.SetActive(true);
        cameraPosition.transform.position -= new Vector3(0f, 0.8f, 0f);
    }

    private void ChangeToStandingStance() {
        rigidBodyStandingCapsule.gameObject.SetActive(true);
        rigidBodyCrouchingCapsule.gameObject.SetActive(false);
        cameraPosition.transform.position += new Vector3(0f, 0.8f, 0f);
    }


    private void GameInput_OnSprintStartedAction(object sender, System.EventArgs e) {
        SetMoveState(MoveState.Sprinting);
    }

    private void GameInput_OnSprintCancelledAction(object sender, System.EventArgs e) {
        SetMoveState(MoveState.Walking);
    }

    private void GameInput_OnCrouchAction(object sender, System.EventArgs e) {
        if (currentMoveState != MoveState.Crouching) {
            SetMoveState(MoveState.Crouching);
        }
        else {
            SetMoveState(MoveState.Walking);
        }
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        if (isGrounded && !jumping) {
            Jump();
            jumpTimer = jumpTimerCooldown;
            jumping = true;
        }
    }
    private void Jump() {
        playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleJumpingCooldown() {
        if (jumpTimer <= 0 && (isGrounded || isOnSlope)) {
            jumping = false;
        }

        if (jumpTimer > 0f) {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void HandlePlayerMovement() {
        Transform playerCamera = PlayerCameraController.LocalInstance.GetCameraTransform();
        moveVector = GameInput.Instance.GetMoveForward() * playerCamera.forward + GameInput.Instance.GetMoveRight() * playerCamera.right;
        moveVector.y = 0f;
        moveVector.Normalize();

        if (isOnSlope) {
            moveVector = Vector3.ProjectOnPlane(moveVector, slopeHit.normal).normalized;
            /*
            if (playerRigidBody.velocity.y > 0f) {
                playerRigidBody.AddForce(Vector3.down * 20f, ForceMode.Force);
            }
            */
        }

        playerRigidBody.useGravity = !isOnSlope;


        if (isGrounded) {
            playerRigidBody.AddForce(moveVector * moveForce, ForceMode.Force);
        }
        else {
            playerRigidBody.AddForce(moveVector * moveForce * notGroundedMoveSpeedMultiplier, ForceMode.Force);
        }

        ClimbStep(moveVector);
    }
    
    private void CheckIfPlayerIsGrounded() {
        isGrounded = Physics.CheckSphere(groundPoint.transform.position, 0.4f, walkableLayer);
    }

    private void HandleGroundedRigidBodyDrag() {
        if (isGrounded) {
            playerRigidBody.drag = groundDrag;
        }
        else {
            playerRigidBody.drag = 0;
        }
    }

    private void CheckIfPlayerIsOnSlope() {
        if (Physics.Raycast(groundPoint.transform.position, Vector3.down, out slopeHit, checkSlopeRaycastRange)) {
            float angleOfGround = Vector3.Angle(Vector3.up, slopeHit.normal);
            isOnSlope = angleOfGround < maxSlopeAngle && angleOfGround != 0;
        }
        else {
            isOnSlope = false;
        }
    }

    private void SpeedControl() {
        if (isOnSlope && !jumping) {
            if (playerRigidBody.velocity.magnitude > currentMaxMoveSpeed) {
                playerRigidBody.velocity = playerRigidBody.velocity.normalized * currentMaxMoveSpeed;
            }
        }
        else {
            Vector3 currentPlayerVelocity = new Vector3(playerRigidBody.velocity.x, 0f, playerRigidBody.velocity.z);
            if (currentPlayerVelocity.magnitude > currentMaxMoveSpeed) {
                Vector3 limitedSpeed = currentPlayerVelocity.normalized * currentMaxMoveSpeed;
                playerRigidBody.velocity = new Vector3(limitedSpeed.x, playerRigidBody.velocity.y, limitedSpeed.z);
            }
        }
    }

    private void ClimbStep(Vector3 moveVector) {
        Vector3 stepRaycastLowerPosition = groundPoint.transform.position;
        Vector3 stepRaycastUpperPosition = new Vector3(stepRaycastLowerPosition.x, stepRaycastLowerPosition.y + stepRaycastDistance, stepRaycastLowerPosition.z);

        if (Physics.Raycast(stepRaycastLowerPosition, moveVector, stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveVector, stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
        else if (Physics.Raycast(stepRaycastLowerPosition, moveVector + new Vector3(1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveVector + new Vector3(1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
        else if (Physics.Raycast(stepRaycastLowerPosition, moveVector + new Vector3(-1.5f, 0, 1), stepRaycastRange, walkableLayer)) {
            if (!Physics.Raycast(stepRaycastUpperPosition, moveVector + new Vector3(-1.5f, 0, 1), stepRaycastRange + 0.1f, walkableLayer)) {
                playerRigidBody.transform.position += new Vector3(0f, stepHeight * Time.deltaTime, 0f);
            }
        }
    }

    public bool IsWalking() {
        if (currentMoveState == MoveState.Walking && moveVector != Vector3.zero) {
            return true;
        }
        return false;
    }

    public bool IsSprinting() {
        if (currentMoveState == MoveState.Sprinting && moveVector != Vector3.zero) {
            return true;
        }
        return false;
    }
}
