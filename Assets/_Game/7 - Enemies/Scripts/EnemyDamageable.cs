using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class EnemyDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthStatsSO healthStats;
    
    private float _currentHealth;
    
    private void Awake()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        _currentHealth = healthStats.MaxHealth;
    }

    public void OnDamageTaken(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0,  healthStats.MaxHealth);
        
        healthStats.OnDamageTaken(damage, _currentHealth);
        
            
        if (_currentHealth == 0)
        {
            healthStats.OnDeath();
        }
    }
}
