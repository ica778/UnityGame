using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : NetworkBehaviour {
    public static PlayerLook Instance { get; private set; }

    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject virtualCamera;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float lookSensitivity = 20f;

    private float lookX;
    private float lookY;

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

    private void Update() {
        if (!IsOwner) {
            return;
        }

        LookInput();
        RotateCamera();
    }

    private void LookInput() {
        lookX = GameInput.Instance.GetLookX();
        lookY = GameInput.Instance.GetLookY();

        yRotation += lookX * lookSensitivity;
        xRotation -= lookY * lookSensitivity;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private void RotateCamera() {
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

/*
 public static PlayerCameraController LocalInstance { get; private set; }

 [SerializeField] private GameObject virtualCamera;
 private float lookSensitivity = 20f;
 private CinemachineVirtualCamera cinemachineVirtualCamera;

 private float mouseX;
 private float mouseY;

 private Quaternion playerHeading;
 private Transform cameraTarget;
 private bool hasTarget = false;

 public override void OnStartNetwork() {
     if (!Owner.IsLocalClient) {
         return;
     }
     LocalInstance = this;
 }

 private void Awake() {

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
 }

 private void FixedUpdate() {
     if (!IsOwner) { 
         return; 
     }

     FollowCameraTarget();
 }

 private void HandleCameraMovement() {
     mouseX -= GameInput.Instance.GetLookY() * lookSensitivity;
     mouseX = Mathf.Clamp(mouseX, -90f, 90f);

     mouseY += GameInput.Instance.GetLookX() * lookSensitivity;

     playerHeading = Quaternion.Euler(mouseX, mouseY, 0);
     transform.rotation = playerHeading;
     cameraTarget.transform.rotation = Quaternion.Euler(0, mouseY, 0);
 }

 private void FollowCameraTarget() {
     if (hasTarget) {
         transform.position = cameraTarget.transform.position;
         //transform.rotation = cameraTarget.transform.rotation;
     }
 }

 public Transform GetCameraTransform() {
     return gameObject.transform;
 }

 public void SetCameraTarget(Transform target) {
     cameraTarget = target;
     hasTarget = true;
 }

 public Quaternion GetCameraQuaternionOnlyYAxis() {
     return new Quaternion(0f, playerHeading.y, 0f, playerHeading.w);
 }

 public Vector3 GetLookDirectionVector() {
     return virtualCamera.transform.forward;
 }
}
 */