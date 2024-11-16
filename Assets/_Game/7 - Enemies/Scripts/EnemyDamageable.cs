using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Fusion;
using UnityEngine;

public class EnemyDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private HealthStatsSO healthStats;
    [SerializeField] private NetworkObject networkObject;
    
    [Networked, OnChangedRender(nameof(HealthUpdate))] private float CurrentHealth { get; set; }
    [Networked] private NetworkBool Initialized { get; set; }

    private bool _isDead;
    private uint _networkId;
    
    public override void Spawned()
    {
        base.Spawned();
        _networkId = networkObject.Id.Raw;
        
        InitializeHealth();
        HealthUpdate();
    }
    
    public void InitializeHealth()
    {
        if(Initialized)
            return;
        
        CurrentHealth = healthStats.MaxHealth;
        Initialized = true;
        
//        Debug.Log($"Current: {CurrentHealth}");
    }

    public void OnDamageTaken(float _)
    {
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void OnDamageTakenRPC(float damage, Vector3 position)
    {
        if (_isDead)
            return;
        
        CurrentHealth -= damage;
        
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0,  healthStats.MaxHealth);
        
        if (CurrentHealth == 0)
        {
            healthStats.OnDeath(_networkId);
            _isDead = true;
            return;
        }
        
        healthStats.OnGotAttacked(position, _networkId);
    }

    private void HealthUpdate()
    {
        Debug.LogError($"Health update: {CurrentHealth} {_networkId}");
        healthStats.OnHealthUpdated(CurrentHealth, _networkId);
    }
}
