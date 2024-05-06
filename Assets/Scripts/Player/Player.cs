using FishNet.Object;
using UnityEngine;

public class Player : NetworkBehaviour {
    [SerializeField] private PlayerLook playerLook;

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
    }


    private void Update() {

    }

    private void LateUpdate() {
        HandleCameraInput();
    }

    private void HandleCameraInput() {
        lookXInput = GameInput.Instance.GetLookX() * Time.deltaTime * lookSensitivity;
        lookYInput = GameInput.Instance.GetLookY() * Time.deltaTime * lookSensitivity;

        playerLook.UpdateWithInput(lookXInput, lookYInput);
        playerLook.RotateCamera();
    }


    public int GetPlayerID() {
        return playerID;
    }

}