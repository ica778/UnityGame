using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanLeverInteractableObject : InteractableObjectBase {
    public event EventHandler OnLeverPulled;

    private void Awake() {
        base.Type = InteractableObjectType.CaravanLever;
    }

    public void Trigger() {
        Debug.Log("TESTING LEVER PULLED ================================");
        OnLeverPulled?.Invoke(this, EventArgs.Empty);
    }
}
