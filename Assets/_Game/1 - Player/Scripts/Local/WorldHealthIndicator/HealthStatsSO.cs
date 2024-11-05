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
    
    public event Action<float, uint> CurrentHealthUpdated;
    public event Action<Vector3, uint> GotAttacked;
    public event Action<uint> Death;

    public void OnHealthUpdated(float currentHealth, uint affectedEntityId)
    {
        CurrentHealthUpdated?.Invoke(currentHealth, affectedEntityId);
    }

    public void OnGotAttacked(Vector3 attackerPosition, uint affectedEntityId)
    {
        GotAttacked?.Invoke(attackerPosition, affectedEntityId);
    }

    public void OnDeath(uint affectedEntityId)
    {
        Death?.Invoke(affectedEntityId);
    }
}
