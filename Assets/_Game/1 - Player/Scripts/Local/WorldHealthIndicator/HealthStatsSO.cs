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
    
    public event Action<float> CurrentHealthUpdated;
    public event Action<Vector3> GotAttacked;
    public event Action Death;

    public void OnHealthUpdated(float currentHealth)
    {
        CurrentHealthUpdated?.Invoke(currentHealth);
    }

    public void OnGotAttacked(Vector3 attackerPosition)
    {
        GotAttacked?.Invoke(attackerPosition);
    }

    public void OnDeath()
    {
        Death?.Invoke();
    }
}
