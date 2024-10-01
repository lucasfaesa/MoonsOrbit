using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldHealthIndicator : MonoBehaviour
{
    [SerializeField] private HealthStatsSO healthStats;
    [Space]
    [SerializeField] private Transform healthPivot;
    
    private void OnEnable()
    {
        healthStats.DamageTaken += OnDamageTaken;
    }

    private void OnDisable()
    {
        healthStats.DamageTaken -= OnDamageTaken;
    }

    private void OnDamageTaken(float damageTaken, float currentHealth)
    {
        healthPivot.DOScaleX(currentHealth/healthStats.MaxHealth, 0.3f).SetEase(Ease.OutSine);
    }
    
}
