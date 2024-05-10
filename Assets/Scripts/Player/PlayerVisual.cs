using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour {
    private MeshRenderer[] meshRenderers;

    private void Update() {
        if (!base.IsOwner) {
            return;
        }

        MakePlayerCharacterInvisibleToOwnCamera();
    }

    private void MakePlayerCharacterInvisibleToOwnCamera() {
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers) {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
}