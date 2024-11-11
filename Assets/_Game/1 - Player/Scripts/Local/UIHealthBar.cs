using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private HealthStatsSO playerHealthStats;
    [Space]
    [SerializeField] private Image healthBarImage;


    private void OnEnable()
    {
        playerHealthStats.CurrentHealthUpdated += HealthUpdated;
    }

    private void OnDisable()
    {
        playerHealthStats.CurrentHealthUpdated -= HealthUpdated;
    }

    private void HealthUpdated(float currentHealth)
    {
        healthBarImage.fillAmount = currentHealth / playerHealthStats.MaxHealth;
    }
    
}
