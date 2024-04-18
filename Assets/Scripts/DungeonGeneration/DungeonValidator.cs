using UnityEngine;

public class DungeonValidator : MonoBehaviour {
    [SerializeField] private LayerMask dungeonValidatorColliderLayer;
    [SerializeField] private BoxCollider[] boxColliders;
    [SerializeField] private RoomHandler room;

    public bool CheckIfSpaceIsClear(RoomConnectorHandler parentRoomConnectorToSpawnFrom, RoomConnectorHandler newRoomConnectorHandler) {
        foreach (BoxCollider collider in boxColliders) {
            // TODO: fix this duplicate code for getting quaternion rotation
            Quaternion rotation = newRoomConnectorHandler.transform.rotation * Quaternion.Euler(0, 180, 0);
            rotation = rotation * parentRoomConnectorToSpawnFrom.transform.rotation;

            Vector3 colliderPosition = rotation * collider.transform.position;
            Vector3 center = room.GetRoomSpawnVector(parentRoomConnectorToSpawnFrom, newRoomConnectorHandler) + colliderPosition;
            Vector3 extents = collider.size / 2f;
            extents -= new Vector3(0.01f, 0.01f, 0.01f);
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