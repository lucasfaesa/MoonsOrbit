using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class BulletTrailBehavior : MonoBehaviour
    {

        private ParticleSystem _impactParticle;
        private Vector3 _targetPoint;
        private bool _hitSomething;
        private Vector3 _hitNormal;

        private float _elapsedTime;
        private Vector3 _startPosition;
        private float _distance;
        private float _duration;

        private bool _initialized;
        
        public void Initialize(Vector3 target, float speed, bool hitSomething, ParticleSystem impactParticle, Vector3 hitNormal)
        {
            _targetPoint = target; 
            _hitSomething = hitSomething;
            _impactParticle = impactParticle;
            _hitNormal = hitNormal;
            
            _elapsedTime = 0;
            _startPosition = this.transform.position;
            _distance = Vector3.Distance(_startPosition, _targetPoint);
            _duration = _distance / speed;

            _initialized = true;
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
                    Instantiate(_impactParticle, _targetPoint, Quaternion.LookRotation(_hitNormal));
                }
                
                this.gameObject.SetActive(false);
            }
        }
    }
}

