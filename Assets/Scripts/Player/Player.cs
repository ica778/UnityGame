using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Player : NetworkBehaviour {
    [SerializeField] private List<Behaviour> clientSideScripts = new List<Behaviour>();
    [SerializeField] private List<GameObject> clientSideObjects = new List<GameObject>();

    public override void OnStartNetwork() {
        PlayerManager.Instance.AddPlayer(Owner, GetComponent<Player>());
        Debug.Log("CLIENT CONNECTED WITH Id: " + Owner.ClientId);

        if (!Owner.IsLocalClient) {
            foreach (Behaviour obj in clientSideScripts) {
                obj.enabled = false;
            }

            foreach (GameObject obj in clientSideObjects) {
                obj.SetActive(false);
            }

            //this.enabled = false;
            return;
        }
    }
}
