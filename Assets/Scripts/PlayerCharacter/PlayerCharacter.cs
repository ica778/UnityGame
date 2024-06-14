using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : ECM2.Character {
    [SerializeField] private float maxSprintSpeed = 20f;

    private bool _isSprinting;
    private bool _sprintInputPressed;

    public void Sprint() {
        _sprintInputPressed = true;
    }

    public void StopSprinting() {
        _sprintInputPressed = false;
    }

    public bool IsSprinting() {
        return _isSprinting;
    }

    private bool CanSprint() {
        return base.IsWalking() && !base.IsCrouched();
    }

    private void CheckSprintInput() {
        if (!_isSprinting && _sprintInputPressed && CanSprint()) {
            _isSprinting = true;
        }
        else if (_isSprinting && (!_sprintInputPressed || !CanSprint())) {
            _isSprinting = false;
        }
    }

    public override float GetMaxSpeed() {
        return _isSprinting ? maxSprintSpeed : base.GetMaxSpeed();
    }

    protected override void OnBeforeSimulationUpdate(float deltaTime) {
        base.OnBeforeSimulationUpdate(deltaTime);

        CheckSprintInput();
    }
}