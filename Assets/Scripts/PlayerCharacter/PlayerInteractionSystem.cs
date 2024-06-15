using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerInteractionSystem : MonoBehaviour {
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private PlayerInventory playerInventoryHandler;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private float interactDistance = 5f;
    private RaycastHit raycastHit;
    private InteractableObjectBase interactableObject;

    private void LateUpdate() {
        DetectInteractableObject();
    }

    private void DetectInteractableObject() {
        if (Physics.Raycast(cameraTarget.transform.position, cameraTarget.transform.forward, out raycastHit, interactDistance, interactableLayer | obstacleLayer)) {
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
                case InteractableObjectType.CaravanLever:
                    (interactableObject as CaravanLever)?.Trigger();
                    break;
            }
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        InteractWithObject();
    }

    private void OnEnable() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void OnDisable() {
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
    }
}