using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnJumpAction;
    public event EventHandler OnCrouchAction;
    public event EventHandler OnSprintStartedAction;
    public event EventHandler OnSprintCancelledAction;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSprintCancelledAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSprintStartedAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnCrouchAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public float GetMouseX() {
        return playerInputActions.Player.MouseX.ReadValue<float>() * Time.deltaTime;
    }

    public float GetMouseY() {
        return playerInputActions.Player.MouseY.ReadValue<float>() * Time.deltaTime;
    }

    public float GetMoveForward() {
        return playerInputActions.Player.MoveForward.ReadValue<float>();
    }

    public float GetMoveRight() {
        return playerInputActions.Player.MoveRight.ReadValue<float>();
    }
}
