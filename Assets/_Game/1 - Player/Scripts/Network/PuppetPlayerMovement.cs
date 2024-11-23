using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class PuppetPlayerMovement : NetworkBehaviour
    {
        private static readonly int XMovement = Animator.StringToHash("XMovement");
        private static readonly int YMovement = Animator.StringToHash("YMovement");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        [Header("Components")] 
        [SerializeField] private HealthStatsSO playerHealthStats;
        [SerializeField] private Transform yawTransform;
        [SerializeField] private Transform pitchTransform;
        [SerializeField] private Animator animator;
        
        private float _smoothTime = 10f;
        private float _lerpThreshold = 0.01f; // Threshold to snap to the target
        
        private float _currentXMovement = 0f;
        private float _currentYMovement = 0f;

        private bool _isDead;
        
        public override void Spawned()
        {
            base.Spawned();
            
            playerHealthStats.Death += OnDeath;
            playerHealthStats.Respawn += OnRespawn;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            
            playerHealthStats.Death -= OnDeath;
            playerHealthStats.Respawn -= OnRespawn;
        }

        private void OnDeath(uint u)
        {
            Debug.Log("Death");
            _isDead = true;
            animator.SetBool(IsDead, true);
        }

        private void OnRespawn()
        {
            _isDead = false;
            animator.SetBool(IsDead, false);
        }
        
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!GetInput<PuppetPlayerInputData>(out var inputData) || _isDead)
                return;

            UpdatePositionAndRotation(inputData);

            LerpMovementAnimation(inputData.PlayerInputDirection.x, inputData.PlayerInputDirection.y);
            
            animator.SetBool(IsGrounded, inputData.IsGrounded);
        }

        private void UpdatePositionAndRotation(PuppetPlayerInputData inputData)
        {
            this.transform.position =  inputData.PlayerTransformNetworkData.PlayerModelVisualPos;
            
            yawTransform.rotation = inputData.PlayerTransformNetworkData.PlayerModelVisualRot;
            pitchTransform.localRotation = inputData.PlayerTransformNetworkData.PlayerCameraLocalRot;
        }
        
        private void LerpMovementAnimation(float xInputData, float yInputData)
        {
            _currentXMovement = Mathf.Lerp(_currentXMovement, xInputData, _smoothTime * Runner.DeltaTime);
            _currentYMovement = Mathf.Lerp(_currentYMovement, yInputData, _smoothTime * Runner.DeltaTime);
            
            // Check if current movement is close enough to the target, then snap to target
            if (Mathf.Abs(_currentXMovement - xInputData) < _lerpThreshold)
                _currentXMovement = xInputData;
            
            if (Mathf.Abs(_currentYMovement - yInputData) < _lerpThreshold)
                _currentYMovement = yInputData;
            
            animator.SetFloat(XMovement, _currentXMovement);
            animator.SetFloat(YMovement, _currentYMovement);
        }
        
        
    }
    
   
}

