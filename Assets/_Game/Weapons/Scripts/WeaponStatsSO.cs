using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/Weapon/WeaponStats")]
public class WeaponStatsSO : ScriptableObject
{
    [field:SerializeField] public float Damage { get; private set; }
    [field:SerializeField] public float DelayBetweenShots { get; private set; }
    [field:SerializeField] public float ReloadTime { get; private set; }
}
