using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCameraController : MonoBehaviour {

    [SerializeField] private float lookSensitivity = 500f;
    [SerializeField] private Transform cameraMountTarget;

    private float mouseRotationX;
    private float mouseRotationY;

    private void Awake () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update () {
        // get mouse input
        float mouseInputX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSensitivity;
        float mouseInputY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSensitivity;

        mouseRotationX -= mouseInputY;
        mouseRotationX = Mathf.Clamp(mouseRotationX, -90f, 90f);

        mouseRotationY += mouseInputX;

        transform.rotation = Quaternion.Euler(mouseRotationX, mouseRotationY, 0);

        transform.position = cameraMountTarget.position;
    }
}
