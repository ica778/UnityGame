using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanLever : InteractableObjectBase {

    private void Awake() {
        base.Type = InteractableObjectType.CaravanLever;
    }

    public void Trigger() {
        GameSceneManager.Instance.OnCaravanLeverPulled();
    }
}
