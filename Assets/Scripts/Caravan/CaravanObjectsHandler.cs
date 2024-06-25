using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanObjectsHandler : MonoBehaviour {
    [SerializeField] private BoxCollider interiorCollider;
    [SerializeField] private Transform caravanContentsParent;

    private void OnTriggerEnter(Collider other) {
        Transform obj = GetTopmostParent(other.transform);
        obj.SetParent(caravanContentsParent);
    }

    private void OnTriggerExit(Collider other) {
        Transform obj = GetTopmostParent(other.transform);
        obj.SetParent(null);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    private Transform GetTopmostParent(Transform obj) {
        Transform current = obj;
        while (current.parent != null && current.parent != caravanContentsParent) {
            current = current.parent;
        }
        return current;
    }
}