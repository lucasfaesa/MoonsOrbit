using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthEventsChannel", menuName = "ScriptableObjects/Health/HealthEventsChannel")]
public class HealthStatsSO : ScriptableObject
{
    [field:SerializeField] public float MaxHealth { get; set; } = 100f;
    [field: ReadOnly][field:SerializeField] public float CurrentHealth { get; set; }
    
    public event Action<float, float> DamageTaken;
    public event Action Death;
    
    public void OnDamageTaken(float damage, float currentHealth)
    {
        DamageTaken?.Invoke(damage, currentHealth);
    }

    public void OnDeath()
    {
        Death?.Invoke();
    }
}
