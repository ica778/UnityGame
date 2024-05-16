using UnityEngine;

public class InteractionSystem : MonoBehaviour {
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private float interactDistance = 5f;
    private RaycastHit raycastHit;
    private InteractableObjectBase interactableObject;

    private void LateUpdate() {
        DetectInteractableObject();
    }

    private void DetectInteractableObject() {
        if (Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out raycastHit, interactDistance, interactableLayer | obstacleLayer)) {
            GameObject tempObject = raycastHit.collider.gameObject;

            if ((interactableLayer.value & (1 << tempObject.layer)) != 0) {
                interactableObject = tempObject.GetComponent<InteractableObjectBase>();
                return;
            }
        }

        interactableObject = null;
    }

    public void InteractWithObject() {
        if (interactableObject) {
            switch (interactableObject.Type) {
                case InteractableObjectType.GroundLoot:
                    playerInventoryHandler.PickupGroundLoot((GroundLoot)interactableObject);
                    break;
            }
        }
    }
}