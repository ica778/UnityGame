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

    [SerializeField] private LayerMask targetLayer;

    private float changePositionDelay = 0.2f;
    private float changePositionTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private State currentState = State.Idle;
    private Transform enemyTransform;
    private float targetDistance = 10f;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyTransform = transform;
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
        Transform targetTransform = null;

        Collider[] collidersInRange = Physics.OverlapSphere(enemyTransform.position, targetDistance, targetLayer);
        if (collidersInRange.Length > 0) {
            targetTransform = collidersInRange[0].transform;
        }

        switch (currentState) {
            case State.Idle:
                if (targetTransform) {
                    currentState = State.Chasing;
                    Debug.Log("TESTING STATE CHANGED TO CHASING");
                }

                break;
            case State.Chasing:
                if (!targetTransform) {
                    currentState = State.Idle;
                    Debug.Log("TESTING STATE CHANGED TO IDLE");
                }
                else {
                    navMeshAgent.SetDestination(targetTransform.position);
                }

                break;
        }
    }

}