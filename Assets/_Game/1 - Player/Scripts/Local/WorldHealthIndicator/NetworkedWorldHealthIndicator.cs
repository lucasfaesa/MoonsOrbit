using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class NetworkedWorldHealthIndicator : MonoBehaviour
{
    [SerializeField] private HealthStatsSO healthStats;
    [SerializeField] private NetworkObject networkObject;
    [Space]
    [SerializeField] private Transform healthPivot;
    
    private void OnEnable()
    {
        Debug.LogError($"Subscribed:");
        healthStats.CurrentHealthUpdatedNetworked += OnHealthUpdated;
    }

    private void OnDisable()
    {
        healthStats.CurrentHealthUpdatedNetworked -= OnHealthUpdated;
    }

    private void OnHealthUpdated(float currentHealth, uint networkId)
    {
        Debug.LogError($"Listened: {currentHealth} {networkId}");
        if (networkObject != null && networkObject.Id.Raw != networkId)
            return;
        
        healthPivot.DOScaleX(currentHealth/healthStats.MaxHealth, 0.3f).SetEase(Ease.OutSine);
    }
    
}
