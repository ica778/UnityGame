using FishNet.Object;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanManager : MonoBehaviour {
    [SerializeField] private CaravanAnimations caravanAnimations;

    public void LeverPulled() {
        caravanAnimations.MoveCaravan();
    }
}