using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using Networking;
using UnityEngine;

public class PuppetPlayerWorldHealthIndicator : NetworkBehaviour
{
    [SerializeField] private HealthStatsSO playerHealthStats;
    [SerializeField] private Transform healthPivot;
    [Space]
    [SerializeField] private float lerpSpeed = 5f;
    
    private float _currentHealthScaleX;

    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        if (!GetInput<PuppetPlayerInputData>(out var inputData))
            return;
        
        // Calculate the target scale based on current health
        float targetHealthScaleX = inputData.HealthNetworkData.CurrentHealth / playerHealthStats.MaxHealth;

        // Linearly interpolate the current scale to the target scale over time
        _currentHealthScaleX = Mathf.Lerp(_currentHealthScaleX, targetHealthScaleX, Runner.DeltaTime * lerpSpeed);

        // Apply the updated scale to the health bar
        healthPivot.localScale = new Vector3(_currentHealthScaleX, healthPivot.localScale.y, healthPivot.localScale.z);
    }

    private void Update()
    {
        if(Camera.main != null)
            this.transform.LookAt(Camera.main.transform);
    }
}
