using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour {
    [SerializeField] private int health;

    private int Health {
        get { return health; }
        set {
            health = value;
            CheckIfCharacterDead();
        }
    }

    private void CheckIfCharacterDead() {
        if (health <= 0) {
            Debug.Log("TESTING: CHARACTER IS DEAD");
        }
    }

    public void ApplyDamage(int damage) {
        Health -= damage;
    }
}