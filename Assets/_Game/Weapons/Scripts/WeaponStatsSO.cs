using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.FPS.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/Weapon/WeaponStats")]
public class WeaponStatsSO : ScriptableObject
{
    [field:SerializeField] public float Damage { get; private set; }
    [field:SerializeField] public float DelayBetweenShots { get; private set; }
    [field:SerializeField] public float ReloadTime { get; private set; }
    
    [field:SerializeField] public bool HasBulletSpread { get; private set; }
    [field:ShowIf("HasBulletSpread")] [field: SerializeField] public Vector3 BulletSpreadVariance { get; private set; } = new Vector3(0.1f, 0.1f, 0.1f);

    [field: SerializeField] public ParticleSystem MuzzleFlashParticle { get; private set; }
    [field: SerializeField] public ParticleSystem ImpactParticle { get; private set; }
    [field: SerializeField] public ProjectileBase BulletTrail { get; private set; }
    [field: SerializeField] public float TrailSpeed { get; private set; } = 15f;

    [field: SerializeField] public LayerMask Mask { get; private set; }
    


}
