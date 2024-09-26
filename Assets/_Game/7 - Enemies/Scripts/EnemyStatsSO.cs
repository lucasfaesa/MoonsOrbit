using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/Enemy/EnemyStats")]
    public class EnemyStatsSO : ScriptableObject
    {
        [field:SerializeField] public float IdleTime { get; private set; } = 2.0f;
        [field:SerializeField] public float DetectionDistance { get; private set; } = 12f;
        [field:SerializeField] public float AttackDistance { get; private set; } = 5f;
        [field:SerializeField] public float PathUpdateDelay { get; private set; } = 0.2f;
        [field:Space]
        [field:SerializeField] public float BulletSpeed { get; private set; } = 200f;
        [field:SerializeField] public LayerMask ShootingLayerMask { get; private set; }
        [field:SerializeField] public Vector3 BulletSpread { get; private set; }
        [field:Space]
        [field:SerializeField] public float RotationRate { get; private set; } = 0.2f;
        
    }
}