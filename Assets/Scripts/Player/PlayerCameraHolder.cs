using FishNet.Object;
using UnityEngine;

public class PlayerCameraHolder : NetworkBehaviour {
    [SerializeField] private Transform playerCharacterCameraPosition;

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            this.enabled = false;
        }
    }

    private void LateUpdate() {
        transform.position = playerCharacterCameraPosition.position;
    }
}