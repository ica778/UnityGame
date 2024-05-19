using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

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
    private Transform currentTransform;

    private Vector3[] visionRaycastPositions = {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, 0.4f, 0),
        new Vector3(0, -0.4f, 0),
    };

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentTransform = transform;
        this.enabled = false;
    }

    // This makes AI script run only on the host
    public override void OnStartClient() {
        EnableScriptForServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableScriptForServerRpc() {
        if (base.IsHost) {
            this.enabled = true;
        }
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

    private void EnemyBehaviour() {
        Transform detectedTargetTransform = DetectTarget();

        switch (currentState) {
            case State.Idle:
                if (detectedTargetTransform) {
                    currentState = State.Chasing;
                }
                break;
            case State.Chasing:
                if (!detectedTargetTransform) {
                    currentState = State.Idle;
                }
                else {
                    navMeshAgent.SetDestination(detectedTargetTransform.position);
                }
                break;
        }
    }

    private Vector3 GetDirectionToTarget(Vector3 targetPosition) {
        return (targetPosition - currentTransform.position);
    }

    // TODO: check if this is more efficient than just raycasting to range of overlap sphere radius
    private float GetDistanceToTarget(Vector3 targetPosition) {
        return Vector3.Distance(currentTransform.position, targetPosition);
    }

    private bool CheckIfTargetIsInFieldOfVision(Vector3 directionToTarget) {
        float angleToTarget = Vector3.Angle(currentTransform.forward, directionToTarget);
        if (angleToTarget < (viewAngle / 2)) {
            return true;
        }
        return false;
    }

    private bool CheckIfTargetIsVisible(Vector3 targetPosition) {
        Vector3 directionToTarget = GetDirectionToTarget(targetPosition);
        if (!CheckIfTargetIsInFieldOfVision(directionToTarget)) {
            return false;
        }

        float distanceToTarget = GetDistanceToTarget(targetPosition);
        RaycastHit hitInfo;

        // NOTE: drawray for testing
        Debug.DrawRay(currentTransform.position, directionToTarget, Color.green, 1f);

        if (Physics.Raycast(currentTransform.position, directionToTarget, out hitInfo, distanceToTarget, targetLayer | obstacleLayer)) {
            GameObject hitObject = hitInfo.collider.gameObject;

            if ((targetLayer.value & (1 << hitObject.layer)) != 0) {
                return true;
            }
        }

        return false;
    }

    // TODO: make dedicated hitbox for detection for the player
    private Transform DetectTarget() {
        Collider[] collidersInRange = Physics.OverlapSphere(currentTransform.position, detectionRadius, targetLayer);
        List<Collider> sortedColliders = collidersInRange.OrderBy(collider => Vector3.Distance(collider.transform.position, currentTransform.position)).ToList();

        foreach (Collider collider in sortedColliders) {
            Transform currentTargetTransform = collider.transform;

            // this checks if target detectable by vision
            foreach (Vector3 pos in visionRaycastPositions) {
                if (CheckIfTargetIsVisible(currentTargetTransform.position + pos)) {
                    // target is detected with this raycast
                    return currentTargetTransform;
                }
            }
        }
        return null;
    }
}