using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnJumpStartedAction;
    public event EventHandler OnJumpCanceledAction;
    public event EventHandler OnCrouchStartedAction;
    public event EventHandler OnCrouchCanceledAction;
    public event EventHandler OnSprintStartedAction;
    public event EventHandler OnSprintCanceledAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnInventoryAction;
    public event EventHandler OnDropAction;
    public event EventHandler OnScrollUpAction;
    public event EventHandler OnScrollDownAction;
    public event EventHandler OnLeftClickPressedAction;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.started += Jump_started;
        playerInputActions.Player.Jump.canceled += Jump_canceled;

        playerInputActions.Player.Crouch.started += Crouch_started;
        playerInputActions.Player.Crouch.canceled += Crouch_canceled;

        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;

        playerInputActions.Player.Pause.performed += Pause_performed;

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

        playerInputActions.Player.Inventory.performed += Inventory_performed;
        playerInputActions.Player.Drop.performed += Drop_performed;

        playerInputActions.Player.Scrolling.performed += Scrolling_performed;
        playerInputActions.Player.LeftClick.performed += LeftClick_performed;
    }

    private void LeftClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnLeftClickPressedAction?.Invoke(this, EventArgs.Empty);
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

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSprintCanceledAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSprintStartedAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnCrouchStartedAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnCrouchCanceledAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpStartedAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpCanceledAction?.Invoke(this, EventArgs.Empty);
    }

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public float GetLookX() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            return playerInputActions.Player.Look.ReadValue<Vector2>().x;
        }
        else {
            return 0f;
        }
        //return playerInputActions.Player.Look.ReadValue<Vector2>().x;
    }

    public float GetLookY() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            return playerInputActions.Player.Look.ReadValue<Vector2>().y;
        }
        else {
            return 0f;
        }
        //return playerInputActions.Player.Look.ReadValue<Vector2>().y;
    }
    public Vector2 GetMoveVector() {
        return playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    private void OnDestroy() {
        playerInputActions.Player.Jump.started -= Jump_started;
        playerInputActions.Player.Jump.canceled -= Jump_canceled;
        playerInputActions.Player.Crouch.started -= Crouch_started;
        playerInputActions.Player.Crouch.canceled -= Crouch_canceled;
        playerInputActions.Player.Sprint.started -= Sprint_started;
        playerInputActions.Player.Sprint.canceled -= Sprint_canceled;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Inventory.performed -= Inventory_performed;
        playerInputActions.Player.Drop.performed -= Drop_performed;

        playerInputActions.Player.Scrolling.performed -= Scrolling_performed;
        playerInputActions.Player.LeftClick.performed -= LeftClick_performed;

        playerInputActions.Dispose();
    }
}
