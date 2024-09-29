using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    
    public void TakeDamage()
    {
        maxHealth -= 10f;
    }
}
