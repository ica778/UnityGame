using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : NetworkBehaviour {
    public static PlayerLook Instance { get; private set; }

    // NOTE: playerCameraHolder is its own gameobject atm
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject virtualCamera;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float xRotation;
    private float yRotation;

    public override void OnStartNetwork() {
        if (!Owner.IsLocalClient) {
            return;
        }

        Instance = this;
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

    public void UpdateWithInput(float lookXInput, float lookYInput) {
        yRotation += lookXInput;
        xRotation -= lookYInput;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public void RotateCamera() {
        playerCameraHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public Quaternion GetCameraQuaternionOnlyYAxis() {
        return Quaternion.Euler(0, yRotation, 0);
    }

    public Vector3 GetLookDirectionVector() {
        return playerCameraHolder.transform.forward;
    }

    public Transform GetCameraTransform() {
        return playerCameraHolder.transform;
    }

}