using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class WorldHealthIndicator : MonoBehaviour
{
    [SerializeField] private HealthStatsSO healthStats;
    [SerializeField] private NetworkObject networkObject;
    [Space]
    [SerializeField] private Transform healthPivot;
    
    private void OnEnable()
    {
        healthStats.CurrentHealthUpdated += OnHealthUpdated;
    }

    private void OnDisable()
    {
        healthStats.CurrentHealthUpdated -= OnHealthUpdated;
    }

    private void OnHealthUpdated(float currentHealth, uint networkId)
    {
        if (networkObject.Id.Raw != networkId)
            return;
        
        healthPivot.DOScaleX(currentHealth/healthStats.MaxHealth, 0.3f).SetEase(Ease.OutSine);
    }
    
}
