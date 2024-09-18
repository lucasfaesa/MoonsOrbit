using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponBulletIndicator : MonoBehaviour
{
    [SerializeField] private GunStatusChannelSO gunStatusChannel;
    [Space] 
    [SerializeField] private TextMeshPro bulletsText;
    
    private void OnEnable()
    {
        gunStatusChannel.CurrentBulletsAmountChanged += OnCurrentBulletAmountChanged;
    }

    private void OnDisable()
    {
        gunStatusChannel.CurrentBulletsAmountChanged -= OnCurrentBulletAmountChanged;
    }

    private void OnCurrentBulletAmountChanged(int bullets)
    {
        bulletsText.text = bullets.ToString();
    }
}
