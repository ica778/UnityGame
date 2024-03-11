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
    public event EventHandler OnPauseAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInventoryAction;
    public event EventHandler OnDropAction;
    public event EventHandler OnScrollUpAction;
    public event EventHandler OnScrollDownAction;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;
        playerInputActions.Player.Pause.performed += Pause_performed;

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Inventory.performed += Inventory_performed;
        playerInputActions.Player.Drop.performed += Drop_performed;

        playerInputActions.Player.Scrolling.performed += Scrolling_performed;
    }

    private void Scrolling_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        float scrollValue = playerInputActions.Player.Scrolling.ReadValue<float>();
        if (scrollValue > 0f) {
            OnScrollUpAction?.Invoke(this, EventArgs.Empty);
        }
        else if (scrollValue < 0f) {
            OnScrollDownAction?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Drop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnDropAction?.Invoke(this, EventArgs.Empty);
    }

    private void Inventory_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInventoryAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
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

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public float GetMouseX() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            return playerInputActions.Player.MouseX.ReadValue<float>() * Time.deltaTime;
        }
        else {
            return 0f;
        }
    }

    public float GetMouseY() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            return playerInputActions.Player.MouseY.ReadValue<float>() * Time.deltaTime;
        }
        else {
            return 0f;
        }
    }

    public float GetMoveForward() {
        return playerInputActions.Player.MoveForward.ReadValue<float>();
    }

    public float GetMoveRight() {
        return playerInputActions.Player.MoveRight.ReadValue<float>();
    }
}
