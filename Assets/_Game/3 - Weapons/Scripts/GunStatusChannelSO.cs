using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GunStatusChannel", menuName = "ScriptableObjects/Weapon/GunStatusChannel")]
public class GunStatusChannelSO : ScriptableObject
{
    private int _currentBulletsAmount;
    public int CurrentBulletsAmount
    {
        get => _currentBulletsAmount;
        set
        {
            _currentBulletsAmount = value;
            OnCurrentBulletsAmountChanged();
        }
    }

    public event Action<int> CurrentBulletsAmountChanged;
    public event Action<bool> IsAiming;

    private void OnCurrentBulletsAmountChanged()
    {
        CurrentBulletsAmountChanged?.Invoke(CurrentBulletsAmount);
    }

    public void OnIsAiming(bool status)
    {
        IsAiming?.Invoke(status);
    }
}
