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
        healthStats.CurrentHealthUpdated += OnHealthUpdated;
    }

    private void OnDisable()
    {
        healthStats.CurrentHealthUpdated -= OnHealthUpdated;
    }

    private void OnHealthUpdated(float currentHealth)
    {
        healthPivot.DOScaleX(currentHealth/healthStats.MaxHealth, 0.3f).SetEase(Ease.OutSine);
    }
    
}
