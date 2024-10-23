using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class HurtEffectController : MonoBehaviour
{
    [SerializeField] private HealthStatsSO playerHealthStats;
    [Space]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject brokenGlassImg;
    [Space] 
    [SerializeField] private float fadeInTime = 0.1f;
    [SerializeField] private float fadeOutTime = 0.3f;

    private Sequence fadeSequence;
    
    private void OnEnable()
    {
        playerHealthStats.GotAttacked += OnGotAttacked;
    }

    private void OnDisable()
    {
        playerHealthStats.GotAttacked -= OnGotAttacked;
    }

    private void Start()
    {
        fadeSequence = DOTween.Sequence()
                            .Append(canvasGroup.DOFade(1f, fadeInTime))
                            .Append(canvasGroup.DOFade(0f, fadeOutTime))
                            .OnComplete(() =>
                            {
                                brokenGlassImg.SetActive(false);
                            })
                            .SetAutoKill(false)
                            .Pause();
    }

    private void OnGotAttacked(Vector3 _)
    {
        if (fadeSequence.IsPlaying())
            return;
        
        canvasGroup.alpha = 0f;
        
        brokenGlassImg.SetActive(true);
        
        fadeSequence.Restart();
    }

    [Button]
    private void debug()
    {
        OnGotAttacked(Vector3.down);
    }
}
