using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Playables;

public class CaravanMovingPlatform : MonoBehaviour, IMoverController {
    public PhysicsMover Mover;
    public Animator animator;
    private Transform _transform;

    private void Start() {
        _transform = this.transform;

        Mover.MoverController = this;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
        // Remember pose before animation
        Vector3 _positionBeforeAnim = _transform.position;
        Quaternion _rotationBeforeAnim = _transform.rotation;

        // Update animation
        animator.Update(0);

        // Set our platform's goal pose to the animation's
        goalPosition = _transform.position;
        goalRotation = _transform.rotation;

        // Reset the actual transform pose to where it was before evaluating. 
        // This is so that the real movement can be handled by the physics mover; not the animation
        _transform.position = _positionBeforeAnim;
        _transform.rotation = _rotationBeforeAnim;
    }
}
