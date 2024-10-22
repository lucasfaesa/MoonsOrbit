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
    [Networked] private NetworkBool Initialized { get; set; }
    
    public override void Spawned()
    {
        base.Spawned();
        InitializeHealth();
        HealthUpdate();
    }
    
    public void InitializeHealth()
    {
        if(Initialized)
            return;
        
        CurrentHealth = healthStats.MaxHealth;
        Initialized = true;
        
        Debug.Log($"Current: {CurrentHealth}");
    }

    public void OnDamageTaken(float _)
    {
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void OnDamageTakenRPC(float damage, Vector3 position)
    {
        CurrentHealth -= damage;
        
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0,  healthStats.MaxHealth);
        
        if (CurrentHealth == 0)
        {
            healthStats.OnDeath();
        }
        
        healthStats.OnGotAttacked(position);
    }

    private void HealthUpdate()
    {
        healthStats.OnHealthUpdated(CurrentHealth);
    }
}
