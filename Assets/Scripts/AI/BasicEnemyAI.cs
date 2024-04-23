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
        Transform targetInOverlapSphereTransform = null;
        bool targetDetected = false;

        Collider[] collidersInRange = Physics.OverlapSphere(enemyTransform.position, detectionRadius, targetLayer);
        // TODO: no consistent way of selecting a certain player if more than one in range
        foreach (Collider collider in collidersInRange) {
            // TODO: optimize by caching the transforms for each player collider
            targetInOverlapSphereTransform = collider.transform;

            targetDetected = CheckIfTargetDetectableByVision(targetInOverlapSphereTransform);

            if (targetDetected) {
                Debug.Log("TESTING TARGET DETECTED: " + targetInOverlapSphereTransform.position);
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
                    //navMeshAgent.SetDestination(targetInRangeTransform.position);
                }

                break;
        }
    }

    // TODO: make it check other parts of player by modifying the directionToTarget and targetAngle so that it raycasts other parts of the player. probably
    // have to wait till player dimensions are finalized first though unless you want to make it really expensive and check a bunch of points. Have to make it
    // check other points so if player's feet is sticking out or something the enemy can still see it.
    private bool CheckIfTargetDetectableByVision(Transform targetInOverlapSphereTransform) {
        Vector3 directionToTarget = (targetInOverlapSphereTransform.position - enemyTransform.position).normalized;
        float targetAngle = Vector3.Angle(enemyTransform.forward, directionToTarget);

        if (targetAngle < (viewAngle / 2)) {
            float distanceToTarget = Vector3.Distance(enemyTransform.position, targetInOverlapSphereTransform.position);

            if (distanceToTarget <= detectionRadius) {
                if (!Physics.Raycast(enemyTransform.position, directionToTarget, distanceToTarget, obstacleLayer)) {
                    return true;
                }
            }
        }
        return false;
    }

}