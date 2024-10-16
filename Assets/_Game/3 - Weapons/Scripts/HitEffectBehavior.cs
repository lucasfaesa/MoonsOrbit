using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HitEffectBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    
    private IObjectPool<ParticleSystem> _thisObjectPool;
    public IObjectPool<ParticleSystem> ObjectPool { set => _thisObjectPool = value; }

    public bool SetToDestroyItself { get; set; }
    
    private void OnParticleSystemStopped()
    {
        this.transform.parent = null;
        _thisObjectPool.Release(particleSystem);
        
        if (SetToDestroyItself)
            Destroy(this.gameObject);
        
    }
    
}
