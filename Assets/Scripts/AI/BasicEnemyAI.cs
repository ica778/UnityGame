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
    [SerializeField] private LayerMask obstacleLayer;

    private float detectionRadius = 20f;
    private float viewAngle = 120f;

    private float changePositionDelay = 0.2f;
    private float changePositionTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private State currentState = State.Idle;
    private Transform enemyTransform;

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
        Transform targetInRangeTransform = null;
        bool targetDetected = false;

        Collider[] collidersInRange = Physics.OverlapSphere(enemyTransform.position, detectionRadius, targetLayer);
        foreach (Collider collider in collidersInRange) {
            // TODO: optimize by caching the transforms for each player collider
            targetInRangeTransform = collider.transform;

            targetDetected = CheckIfTargetDetectableByVision(targetInRangeTransform);

            if (targetDetected) {
                break;
            }
        }

        switch (currentState) {
            case State.Idle:
                if (targetDetected) {
                    currentState = State.Chasing;
                    Debug.Log("TESTING STATE CHANGED TO CHASING");
                }

                break;
            case State.Chasing:
                if (!targetDetected) {
                    currentState = State.Idle;
                    Debug.Log("TESTING STATE CHANGED TO IDLE");
                }
                else {
                    navMeshAgent.SetDestination(targetInRangeTransform.position);
                }

                break;
        }
    }

    private bool CheckIfTargetDetectableByVision(Transform targetInRangeTransform) {
        Vector3 directionToTarget = (targetInRangeTransform.position - enemyTransform.position).normalized;
        float targetAngle = Vector3.Angle(enemyTransform.forward, directionToTarget);

        if (targetAngle < (viewAngle / 2)) {
            float distanceToTarget = Vector3.Distance(enemyTransform.position, targetInRangeTransform.position);
            
            if (distanceToTarget <= detectionRadius) {
                if (!Physics.Raycast(enemyTransform.position, directionToTarget, distanceToTarget, obstacleLayer)) {
                    return true;
                }
            }
        }
        return false;
    }

}