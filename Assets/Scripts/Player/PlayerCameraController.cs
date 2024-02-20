using Cinemachine;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour {

    public static PlayerCameraController Instance { get; private set; }
    
    [SerializeField] private GameObject virtualCamera;
    private float lookSensitivity = 20f;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float mouseX;
    private float mouseY;

    private Quaternion playerHeading;
    private Transform cameraTarget;
    private bool hasTarget = false;

    private void Awake() {
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

    private void Update() {
        if (!IsOwner) {
            return;
        }

        HandleCameraMovement();
        if (hasTarget) {
            transform.position = cameraTarget.transform.position;
        }
    }

    private void HandleCameraMovement() {
        mouseX -= GameInput.Instance.GetMouseY() * lookSensitivity;
        mouseX = Mathf.Clamp(mouseX, -90f, 90f);

        mouseY += GameInput.Instance.GetMouseX() * lookSensitivity;

        playerHeading = Quaternion.Euler(mouseX, mouseY, 0);
        transform.rotation = playerHeading;
    }

    public Transform GetCameraTransform() {
        return gameObject.transform;
    }

    public void SetCameraTarget(Transform target) {
        cameraTarget = target;
        hasTarget = true;
    }
}
