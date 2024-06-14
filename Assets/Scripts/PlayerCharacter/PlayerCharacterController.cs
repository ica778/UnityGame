using ECM2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : Character {

    private void Update() {
        HandleMovementInput();
    }

    private void HandleMovementInput() {
        Vector2 moveVector2D = GameInput.Instance.GetMoveVector();
        Vector3 moveVector3D = new Vector3(moveVector2D.x, 0f, moveVector2D.y);
        moveVector3D = moveVector3D.relativeTo(base.cameraTransform);

        base.SetMovementDirection(moveVector3D);
    }
}
