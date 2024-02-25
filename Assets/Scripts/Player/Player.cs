using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SerializeField] private GameObject playerCharacter;
    private PlayerMovement playerMovement;

    private void Start() {
        PlayerManager.Instance.AddPlayer(base.ObjectId, GetComponent<Player>());
        playerMovement = GetComponentInChildren<PlayerMovement>();
        Debug.Log("CLIENT CONNECTED WITH ID: " + base.ObjectId);
    }

    public PlayerMovement GetPlayerMovement() {
        return playerMovement;
    }
}