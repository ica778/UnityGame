using FishNet;
using FishNet.Object;
using System;
using UnityEngine;

public class CaravanManager : NetworkBehaviour {
    [SerializeField] private CaravanLeverInteractableObject caravanLever;
    [SerializeField] private CaravanMovement caravanMovement;

    [ServerRpc(RequireOwnership = false)]
    private void MoveCaravanServerRpc() {
        caravanMovement.StartMovingCaravan();
    }

    private void CaravanLever_OnLeverPulled(object sender, EventArgs e) {
        MoveCaravanServerRpc();
    }

    private void OnEnable() {
        caravanLever.OnLeverPulled += CaravanLever_OnLeverPulled;
    }

    private void OnDisable() {
        caravanLever.OnLeverPulled -= CaravanLever_OnLeverPulled;
    }
}