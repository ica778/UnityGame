using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    [SerializeField] private BoxCollider[] boxColliders;
    [SerializeField] private RoomHandler room;

    public bool CheckIfSpaceIsClear(RoomConnectorHandler parentRoomConnectorToSpawnFrom) {
        foreach (BoxCollider collider in boxColliders) {
            Quaternion rotation = Quaternion.Inverse(parentRoomConnectorToSpawnFrom.transform.rotation);

            if (rotation.eulerAngles.y == 0 || rotation.eulerAngles.y == 180) {
                rotation *= Quaternion.Euler(0, 180, 0);
            }

            Vector3 colliderPosition = rotation * collider.transform.position;

            Vector3 center = room.GetRoomSpawnVector(parentRoomConnectorToSpawnFrom) + colliderPosition;
            Vector3 extents = collider.size / 2f;
            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, rotation, dungeonValidatorColliderLayer);

            if (overlappingColliders.Length > 0) {
                return false;
            }
        }
        return true;
    }

    public void DisableColliders() {
        foreach (BoxCollider collider in boxColliders) {
            collider.gameObject.SetActive(false);
        }
    }

    public void EnableColliders() {
        foreach (BoxCollider collider in boxColliders) {
            collider.gameObject.SetActive(true);
        }
    }
}