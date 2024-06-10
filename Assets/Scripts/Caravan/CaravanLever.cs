using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanLever : InteractableObjectBase {
    [SerializeField] private CaravanManager caravanManager;

    private void Awake() {
        base.Type = InteractableObjectType.CaravanLever;
    }

    public void Trigger() {
        //GameSceneManager.Instance.OnCaravanLeverPulled();
        caravanManager.LeverPulled();
    }
}
