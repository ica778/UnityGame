using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    [SerializeField] private BoxCollider[] boxColliders;
    [SerializeField] private RoomHandler room;

    public bool CheckIfSpaceIsClear(GameObject roomPrefab, RoomConnectorHandler roomSpawnEntrance) {
        foreach (BoxCollider collider in boxColliders) {
            Vector3 center = room.GetRoomSpawnVector(roomSpawnEntrance) + collider.transform.position;
            Vector3 extents = collider.size / 2f;
            Collider[] overlappingColliders = Physics.OverlapBox(center, extents, roomSpawnEntrance.transform.rotation, dungeonValidatorColliderLayer);
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