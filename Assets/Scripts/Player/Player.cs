using FishNet.Object;
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
            return;
        }

        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;

    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        characterController.RequestJump();
    }

    private void FixedUpdate() {
        HandleCharacterMovementInput();
    }

    private void Update() {

    }

    private void LateUpdate() {
        HandleCameraInput();
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