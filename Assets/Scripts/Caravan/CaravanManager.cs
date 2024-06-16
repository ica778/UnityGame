using FishNet;
using FishNet.Object;
using System;
using UnityEngine;

public class CaravanManager : NetworkBehaviour {
    [SerializeField] private CaravanLeverInteractableObject caravanLever;
    [SerializeField] private CaravanMovement caravanMovement;

    private bool leverAlreadyPulledLock = false;

    [ServerRpc(RequireOwnership = false)]
    public void StartMovingCaravanServerRpc() {
        if (leverAlreadyPulledLock) {
            return;
        }
        
        leverAlreadyPulledLock = true;
        caravanMovement.StartMovingCaravanObserversRpc();
    }

    public void OnCaravanFinishedMoving() {
        leverAlreadyPulledLock = false;
    }

    private void CaravanLever_OnLeverPulled(object sender, EventArgs e) {
        StartMovingCaravanServerRpc();
    }

    private void OnEnable() {
        caravanLever.OnLeverPulled += CaravanLever_OnLeverPulled;
    }

    private void OnDisable() {
        caravanLever.OnLeverPulled -= CaravanLever_OnLeverPulled;
    }
}