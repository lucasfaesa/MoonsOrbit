using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using Networking;
using UnityEngine;
using UnityEngine.Pool;

public class PhysicalBulletBehavior : MonoBehaviour
{
        [SerializeField] private ParticleSystem metalHitEffectPrefab;
        [SerializeField] private ParticleSystem bloodHitEffectPrefab;
        
        private Vector3 _targetPoint;
        
        private float _elapsedTime;
        private Vector3 _startPosition;
        private float _distance;
        private float _duration;

        private bool _initialized;
        
        private IObjectPool<PhysicalBulletBehavior> _thisObjectPool;
        public IObjectPool<PhysicalBulletBehavior> ObjectPool { set => _thisObjectPool = value; }

        private IObjectPool<ParticleSystem> _metalHitEffectPool;
        private IObjectPool<ParticleSystem> _bloodHitEffectPool;
        
        private enum PoolType {METAL, BLOOD}
        
        private void Awake()
        {
            CreatePools();
        }
        
        public void Initialize(Vector3 target, float bulletSpeed)
        {
            _targetPoint = target; 
            
            _elapsedTime = 0;
            _startPosition = this.transform.position;
            _distance = Vector3.Distance(_startPosition, _targetPoint);
            _duration = _distance / bulletSpeed;

            _initialized = true;
        }

        
        private void CreatePools()
        {
            _metalHitEffectPool = CreateEffectPool(metalHitEffectPrefab, 20, 50, PoolType.METAL);
            _bloodHitEffectPool = CreateEffectPool(bloodHitEffectPrefab, 20, 50, PoolType.BLOOD);
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
                
                //_thisObjectPool.Release(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collided with {other.gameObject.name}", other.gameObject);
            var targetType = GetLayerHit(other.gameObject.layer);

            if (targetType == ConstantsManager.TargetType.MONSTER)
                return;

            var impactParticle = SetImpactParticle(targetType);
            
            if (targetType == ConstantsManager.TargetType.HUMAN)
            {
                Debug.Log("Hit Human");
            }
            
            /*RaycastHit hit;
            Vector3 directionToOther = other.transform.position - transform.position;

            if (Physics.Raycast(transform.position, directionToOther, out hit))
            {
                impactParticle.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
                impactParticle.Play();
            }*/
            
            this.gameObject.SetActive(false);
            //_thisObjectPool.Release(this);
        }
        
        private ConstantsManager.TargetType GetLayerHit(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
        
            return layerName switch
            {
                ConstantsManager.PLAYER_HITDETECTION_COLLIDER_LAYER or ConstantsManager.NETWORK_PLAYER_HITDETECTION_COLLIDER_LAYER => ConstantsManager.TargetType.HUMAN,
                ConstantsManager.ENEMY_LAYER => ConstantsManager.TargetType.MONSTER,
                _ => ConstantsManager.TargetType.METAL
            };
        }
}
