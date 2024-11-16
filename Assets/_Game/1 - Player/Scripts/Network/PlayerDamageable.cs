using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthStatsSO healthStats;
    
    private bool _isDead;

    private void Start()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        healthStats.CurrentHealth = healthStats.MaxHealth;
    }

    public void OnDamageTaken(float damage)
    {
        if (_isDead)
            return;
        
        healthStats.CurrentHealth -= damage;
        healthStats.OnHealthUpdated();
        
        if (healthStats.CurrentHealth <= 0)
        {
            healthStats.CurrentHealth = 0;
            OnDeath();
        }
    }

    public void OnDamageTakenRPC(float damage, Vector3 position)
    {
    }

    private void OnDeath()
    {
        healthStats.OnDeath(0);
    }
}
