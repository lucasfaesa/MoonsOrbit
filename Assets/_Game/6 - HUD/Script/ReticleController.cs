using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    [SerializeField] private GunStatusChannelSO gunStatusChannel;
    [Space]
    [SerializeField] private Image reticleImage;

    private void OnEnable()
    {
        gunStatusChannel.IsAiming += OnAiming;
    }

    private void OnDisable()
    {
        gunStatusChannel.IsAiming -= OnAiming;
    }

    private void OnAiming(bool status)
    {
        reticleImage.DOFade(status ? 0f : 1f, 0.1f);
    }
}
