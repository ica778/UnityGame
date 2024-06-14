using ECM2;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour {
    [SerializeField] private ECM2.Character _character;

    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private GameObject standingVirtualCamera;
    [SerializeField] private GameObject crouchingVirtualCamera;

    [SerializeField] private float maxPitch = 89f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float lookSensitivity = 1.5f;

    // Current camera target pitch

    private float _cameraTargetPitch;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        // Disable Character's rotation mode, we'll handle it here

        _character.SetRotationMode(ECM2.Character.RotationMode.None);
    }

    private void Update() {
        DetectMovementInput();
    }

    private void LateUpdate() {
        DetectCameraLookInput();
    }

    private void DetectMovementInput() {
        // Movement input

        Vector2 moveInput = new Vector2 {
            x = GameInput.Instance.GetMoveVector().x,
            y = GameInput.Instance.GetMoveVector().y
        };

        // Movement direction relative to Character's forward

        Vector3 movementDirection = Vector3.zero;

        movementDirection += _character.GetRightVector() * moveInput.x;
        movementDirection += _character.GetForwardVector() * moveInput.y;

        // Set Character movement direction

        _character.SetMovementDirection(movementDirection);
    }

    private void DetectCameraLookInput() {
        // Look input

        Vector2 lookInput = new Vector2 {
            x = GameInput.Instance.GetLookX() * Time.deltaTime,
            y = GameInput.Instance.GetLookY() * Time.deltaTime
        };

        // Add yaw input, this update character's yaw rotation

        AddControlYawInput(lookInput.x * lookSensitivity);

        // Add pitch input (look up / look down), this update cameraTarget's local rotation

        AddControlPitchInput(lookInput.y * lookSensitivity, minPitch, maxPitch);
    }

    /// <summary>
    /// Add input (affecting Yaw).
    /// This is applied to the Character's rotation.
    /// </summary>

    private void AddControlYawInput(float value) {
        _character.AddYawInput(value);
    }

    /// <summary>
    /// Add input (affecting Pitch).
    /// This is applied to the cameraTarget's local rotation.
    /// </summary>

    private void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f) {
        if (value == 0.0f)
            return;

        _cameraTargetPitch = MathLib.ClampAngle(_cameraTargetPitch + value, minValue, maxValue);
        cameraTarget.transform.localRotation = Quaternion.Euler(-_cameraTargetPitch, 0.0f, 0.0f);
    }

    /// <summary>
    /// When character crouches, toggle Crouched / UnCrouched cameras.
    /// </summary>

    private void OnCrouched() {
        crouchingVirtualCamera.SetActive(true);
        standingVirtualCamera.SetActive(false);
    }

    /// <summary>
    /// When character un-crouches, toggle Crouched / UnCrouched cameras.
    /// </summary>

    private void OnUnCrouched() {
        crouchingVirtualCamera.SetActive(false);
        standingVirtualCamera.SetActive(true);
    }

    private void GameInput_OnCrouchStartedAction(object sender, System.EventArgs e) {
        /*
        if (_character.IsCrouched()) {
            _character.UnCrouch();
        }
        else {
            _character.Crouch();
        }
        */
        _character.Crouch();
    }

    private void GameInput_OnCrouchCanceledAction(object sender, System.EventArgs e) {
        _character.UnCrouch();
    }

    private void GameInput_OnJumpStartedAction(object sender, System.EventArgs e) {
        _character.Jump();
    }

    private void GameInput_OnJumpCancelledAction(object sender, System.EventArgs e) {
        _character.StopJumping();
    }

    private void OnEnable() {
        // Subscribe to Character events

        _character.Crouched += OnCrouched;
        _character.UnCrouched += OnUnCrouched;

        GameInput.Instance.OnCrouchStartedAction += GameInput_OnCrouchStartedAction;
        GameInput.Instance.OnCrouchCanceledAction += GameInput_OnCrouchCanceledAction;
        GameInput.Instance.OnJumpStartedAction += GameInput_OnJumpStartedAction;
        GameInput.Instance.OnJumpCanceledAction += GameInput_OnJumpCancelledAction;
    }

    private void OnDisable() {
        // Unsubscribe to Character events

        _character.Crouched -= OnCrouched;
        _character.UnCrouched -= OnUnCrouched;

        GameInput.Instance.OnCrouchStartedAction -= GameInput_OnCrouchStartedAction;
        GameInput.Instance.OnJumpStartedAction -= GameInput_OnJumpStartedAction;
        GameInput.Instance.OnJumpCanceledAction -= GameInput_OnJumpCancelledAction;
    }
}