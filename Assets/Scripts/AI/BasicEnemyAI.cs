using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : NetworkBehaviour {
    private float changePositionDelay = 0.2f;
    private float changePositionTimer = 0f;
    private NavMeshAgent navMeshAgent;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (changePositionTimer >= changePositionDelay) {
            changePositionTimer = 0f;
            ChangePositionServerRpc();
        }
        else {
            changePositionTimer += Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePositionServerRpc() {
        if (PlayerManager.Instance) {
            foreach (KeyValuePair<int, Player> x in PlayerManager.Instance.GetPlayersList()) {
                Transform target = x.Value.GetPlayerCharacter().transform;
                navMeshAgent.SetDestination(target.position);
                return;
            }
            
        }
    }

}