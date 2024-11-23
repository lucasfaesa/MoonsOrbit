using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private HealthStatsSO playerHealth;
    [SerializeField] private CanvasGroup contentCanvasGroup;

    private void OnEnable()
    {
        playerHealth.Death += OnDeath;
        playerHealth.Respawn += OnRespawn;
    }

    private void OnDisable()
    {
        playerHealth.Death -= OnDeath;
        playerHealth.Respawn -= OnRespawn;
    }

    private void OnDeath(uint u)
    {
        contentCanvasGroup.alpha = 0;
        contentCanvasGroup.gameObject.SetActive(true);

        contentCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutSine);
    }

    private void OnRespawn()
    {
        contentCanvasGroup.gameObject.SetActive(false);
    }
}
