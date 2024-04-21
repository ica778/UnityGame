using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : NetworkBehaviour {
    private enum State {
        Idle,
        Chasing,
    }

    private float changePositionDelay = 0.2f;
    private float changePositionTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private State currentState = State.Idle;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (changePositionTimer >= changePositionDelay) {
            changePositionTimer = 0f;
            EnemyBehaviour();
        }
        else {
            changePositionTimer += Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnemyBehaviour() {
        Transform target = PlayerManager.Instance.GetPlayer(3).GetPlayerCharacter().transform;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        switch (currentState) {
            default:
                Debug.LogError("ENEMY INITIAL STATE NOT SET");
                break;
            case State.Idle:
                if (distanceToTarget <= 10f) {
                    currentState = State.Chasing;
                    Debug.Log("TESTING CHASING STATE ACTIVATED");
                }

                break;
            case State.Chasing:
                navMeshAgent.SetDestination(target.position);
                if (distanceToTarget > 10f) {
                    currentState = State.Idle;
                    Debug.Log("TESTING IDLE STATE ACTIVATED");
                }
                break;
        }
    }

}