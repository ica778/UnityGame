using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectType {
    None,
    GroundLoot,
}

public abstract class InteractableObjectBase : MonoBehaviour {

    public InteractableObjectType Type { get; protected set; } = InteractableObjectType.None;
}