using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Fusion;
using UnityEngine;

public class EnemyDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private HealthStatsSO healthStats;
    
    [Networked, OnChangedRender(nameof(HealthUpdate))] private float CurrentHealth { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        InitializeHealth();
        HealthUpdate();
    }
    
    public void InitializeHealth()
    {
        CurrentHealth = healthStats.MaxHealth;
        Debug.Log($"Current: {CurrentHealth}");
    }

    public void OnDamageTaken(float _)
    {
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void OnDamageTakenRPC(float damage)
    {
        CurrentHealth -= damage;
        
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0,  healthStats.MaxHealth);
        
        if (CurrentHealth == 0)
        {
            healthStats.OnDeath();
        }
        
    }

    private void HealthUpdate()
    {
        healthStats.OnHealthUpdated(CurrentHealth);
    }
}
