using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Helpers;
using UnityEngine;
using UnityEngine.Pool;


namespace Networking
{
    public class BulletTrailBehavior : MonoBehaviour
    {

        [Header("Refs")] 
        [SerializeField] private float trailSpeed = 300f;
        [SerializeField] private WeaponStatsSO weaponStats;
        [Space]
        [SerializeField] private ParticleSystem metalHitEffectPrefab;
        [SerializeField] private ParticleSystem bloodHitEffectPrefab;
        [SerializeField] private ParticleSystem goopHitEffectPrefab;
        
        private Vector3 _targetPoint;
        private bool _hitSomething;
        private Vector3 _hitNormal;
        private ParticleSystem _impactParticle;
        
        private float _elapsedTime;
        private Vector3 _startPosition;
        private float _distance;
        private float _duration;

        private bool _initialized;
        
        private IObjectPool<BulletTrailBehavior> _thisObjectPool;
        public IObjectPool<BulletTrailBehavior> ObjectPool { set => _thisObjectPool = value; }

        private IObjectPool<ParticleSystem> _metalHitEffectPool;
        private IObjectPool<ParticleSystem> _bloodHitEffectPool;
        private IObjectPool<ParticleSystem> _goopHitEffectPool;
        
        private enum PoolType {METAL, BLOOD, GOOP}
        
        private void Awake()
        {
            CreatePools();
        }
        
        public void Initialize(Vector3 targetPoint, bool hitSomething, ConstantsManager.TargetType targetType, Vector3 hitNormal)
        {
            _targetPoint = targetPoint;
            _hitSomething = hitSomething;

            _impactParticle = SetImpactParticle(targetType);
            
            _hitNormal = hitNormal;
            
            _elapsedTime = 0;
            _startPosition = this.transform.position;
            _distance = Vector3.Distance(_startPosition, _targetPoint);
            _duration = _distance / trailSpeed;

            _initialized = true;
        }

        
        private void CreatePools()
        {
            _metalHitEffectPool = CreateEffectPool(metalHitEffectPrefab, 20, 50, PoolType.METAL);
            _bloodHitEffectPool = CreateEffectPool(bloodHitEffectPrefab, 20, 50, PoolType.BLOOD);
            _goopHitEffectPool = CreateEffectPool(goopHitEffectPrefab, 20, 50, PoolType.GOOP);
        }

        private IObjectPool<ParticleSystem> CreateEffectPool(ParticleSystem prefab, int defaultCapacity, int maxSize, PoolType type)
        {
            return new ObjectPool<ParticleSystem>(
                createFunc: () =>
                {
                    var instance = Instantiate(prefab);
                    var effectBehavior = instance.GetComponent<HitEffectBehavior>();
                    effectBehavior.ObjectPool = type switch
                    {
                        PoolType.METAL => _metalHitEffectPool,
                        PoolType.BLOOD => _bloodHitEffectPool,
                        PoolType.GOOP => _goopHitEffectPool,
                        _ => _metalHitEffectPool
                    };
                    return instance;
                },
                actionOnGet: effect => effect.gameObject.SetActive(true),
                actionOnRelease: effect => effect.gameObject.SetActive(false),
                actionOnDestroy: effect => Destroy(effect.gameObject),
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private ParticleSystem SetImpactParticle(ConstantsManager.TargetType type)
        {
            if (type == ConstantsManager.TargetType.NONE)
                return null;
            
            return type switch
            {
                ConstantsManager.TargetType.HUMAN => _bloodHitEffectPool.Get(),
                ConstantsManager.TargetType.ENEMY => _goopHitEffectPool.Get(),
                _ => _metalHitEffectPool.Get()
            };
            
        }

        private void Update()
        {
            if (!_initialized) return;
            
            _elapsedTime += Time.deltaTime;
            
            if (_elapsedTime < _duration)
            {
                float t = _elapsedTime / _duration; // Calculate progress
                this.transform.position = Vector3.Lerp(_startPosition, _targetPoint, t);
            }
            else
            {
                this.transform.position = _targetPoint; // Ensure object reaches target
                
                _initialized = false;
                
                if (_hitSomething)
                {
                    _impactParticle.transform.SetPositionAndRotation(_targetPoint, Quaternion.LookRotation(_hitNormal));

                    if (Physics.Raycast(_startPosition, transform.forward, out var hit, _distance + 1f))
                    {
                        var targetType = GetLayerHit(hit.transform.gameObject.layer);
                        
                        if (targetType == ConstantsManager.TargetType.HUMAN)
                        {
                            if (hit.transform.root.TryGetComponent(out IDamageable damageable))
                            {
                                damageable.OnDamageTaken(weaponStats.Damage);
                            }
                        }
                        
                        _impactParticle.transform.SetParent(hit.transform);
                    }
                    
                    _impactParticle.Play();
                }
                
                _thisObjectPool.Release(this);
            }
        }
        
        private ConstantsManager.TargetType GetLayerHit(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
            
            return layerName switch
            {
                ConstantsManager.PLAYER_HITDETECTION_COLLIDER_LAYER 
                    or ConstantsManager.NETWORK_PLAYER_HITDETECTION_COLLIDER_LAYER => ConstantsManager.TargetType.HUMAN,
                
                ConstantsManager.ENEMY_LAYER => ConstantsManager.TargetType.ENEMY,
                
                _ => ConstantsManager.TargetType.METAL
            };
        }
    }
    
}

