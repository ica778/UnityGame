using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class InteractionSystem : MonoBehaviour {
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;

    private float interactDistance = 5f;
    private RaycastHit raycastHit;
    private InteractableObjectBase interactableObject;

    private void LateUpdate() {
        DetectInteractableObject();
    }

    private void DetectInteractableObject() {
        if (Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out raycastHit, interactDistance, interactableLayer)) {
            interactableObject = raycastHit.transform.GetComponent<InteractableObjectBase>();
        }
        else {
            interactableObject = null;
        }

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