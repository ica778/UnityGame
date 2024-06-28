using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Create New Weapon")]
public class WeaponItemSO : ItemSO {
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed;

    public int Damage => damage;

    public float AttackSpeed => attackSpeed;

}
