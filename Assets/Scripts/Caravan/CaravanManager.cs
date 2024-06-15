using UnityEngine;

public class CaravanManager : MonoBehaviour {
    [SerializeField] private CaravanAnimations caravanAnimations;

    public void LeverPulled() {
        caravanAnimations.MoveCaravan();
    }
}