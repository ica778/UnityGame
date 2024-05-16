using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : NetworkBehaviour {
    // NOTE: playerCameraHolder is its own gameobject atm
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private GameObject virtualCamera;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float xRotation;
    private float yRotation;

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            return;
        }
    }

    public override void OnStartClient() {
        cinemachineVirtualCamera = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        if (!base.IsOwner) {
            cinemachineVirtualCamera.Priority = 0;
            return;
        }
        else {
            cinemachineVirtualCamera.Priority = 10;
        }

        GameInput.Instance.LockCursor();
    }

    public void UpdateWithInput(float lookXInput, float lookYInput, float deltaTime) {
        yRotation += lookXInput * deltaTime;
        xRotation -= lookYInput * deltaTime;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public void RotateCamera() {
        playerCameraHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    public float GetYRotation() {
        return yRotation;
    }

    public Quaternion GetCameraQuaternionOnlyYAxis() {
        return Quaternion.Euler(0, yRotation, 0);
    }
}