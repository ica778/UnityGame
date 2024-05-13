using FishNet.Object;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : NetworkBehaviour {
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private MyCharacterController characterController;

    private int playerID;

    private float lookSensitivity = 20f;
    private float lookXInput;
    private float lookYInput;

    private void Start() {
        playerID = base.ObjectId;
        PlayerManager.Instance.AddPlayer(base.ObjectId, GetComponent<Player>());
        Debug.Log("CLIENT CONNECTED WITH ID: " + base.ObjectId);
    }

    public override void OnStartClient() {
        if (!base.IsOwner) {
            KinematicCharacterSystem.UnregisterCharacterMotor(characterController.motor);
            return;
        }

        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnCrouchAction += GameInput_OnCrouchAction;
        GameInput.Instance.OnSprintStartedAction += GameInput_OnSprintStartedAction;
        GameInput.Instance.OnSprintCancelledAction += GameInput_OnSprintCancelledAction;
    }

    private void Update() {
        if (!base.IsOwner) {
            return;
        }

        HandleCharacterMovementInput();
    }

    private void LateUpdate() {
        if (!base.IsOwner) {
            return;
        }

        HandleCameraInput();
    }

    private void GameInput_OnSprintStartedAction(object sender, System.EventArgs e) {
        characterController.RequestSprint();
    }

    private void GameInput_OnSprintCancelledAction(object sender, System.EventArgs e) {
        characterController.RequestWalk();
    }

    private void GameInput_OnCrouchAction(object sender, System.EventArgs e) {
        ToggleCrouchingState();
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        characterController.RequestJump();
    }

    private void ToggleCrouchingState() {
        if (!characterController.isCrouching) {
            characterController.RequestCrouch();
        }
        else {
            characterController.RequestUncrouch();
        }
    }

    private void HandleCameraInput() {
        lookXInput = GameInput.Instance.GetLookX() * lookSensitivity;
        lookYInput = GameInput.Instance.GetLookY() * lookSensitivity;

        playerLook.UpdateWithInput(lookXInput, lookYInput, Time.deltaTime);
        playerLook.RotateCamera();
        characterController.SetLookRotationInput(playerLook.GetYRotation());
    }

    private void HandleCharacterMovementInput() {
        Transform orientation = characterController.transform;
        Vector3 moveDirection = orientation.forward * GameInput.Instance.GetMoveVector().y + orientation.right * GameInput.Instance.GetMoveVector().x;
        characterController.SetMovementVectorInput(moveDirection);
    }

    public int GetPlayerID() {
        return playerID;
    }

}