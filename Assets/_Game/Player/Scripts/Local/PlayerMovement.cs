using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalPlayer
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("SOs")]
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private PlayerStatsSO playerStats;
        [Header("Other")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private CharacterController characterController;


        private Vector3 velocity;
        private bool grounded;

        private void OnEnable()
        {
            inputReader.Jump += OnJump;
            inputReader.EnableInputActions();
        }

        private void OnDisable()
        {
            inputReader.Jump -= OnJump;
            inputReader.DisableInputActions();
        }
        
        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            var direction = inputReader.Direction;
            Vector3 move = playerCamera.transform.right * direction.x
                           + playerCamera.transform.forward * direction.y;

            MoveWithoutRotation(move);
        }

        private void MoveWithoutRotation(Vector3 direction)
        {
            float deltaTime = Time.fixedDeltaTime;
            float tickRate = 1f / Time.fixedDeltaTime; // Assuming you want to match the fixed update rate.
            Vector3 previousPos = transform.position;
            Vector3 moveVelocity = velocity;

            direction = direction.normalized;

            if (grounded && moveVelocity.y < 0)
            {
                moveVelocity.y = 0f;
            }

            moveVelocity.y += playerStats.Gravity * deltaTime;

            Vector3 horizontalVel = new Vector3();
            horizontalVel.x = moveVelocity.x;
            horizontalVel.z = moveVelocity.z;

            if (direction == Vector3.zero)
            {
                horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, playerStats.Braking * deltaTime);
            }
            else
            {
                horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * playerStats.Acceleration * deltaTime,
                    playerStats.MaxSpeed);
            }

            moveVelocity.x = horizontalVel.x;
            moveVelocity.z = horizontalVel.z;

            characterController.Move(moveVelocity * deltaTime);

            velocity = (transform.position - previousPos) * tickRate;
            grounded = characterController.isGrounded;
        }

        private void OnJump(bool jumpPressed)
        {
            if (jumpPressed && grounded)
                Jump();
        }

        private void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
        {
            if (grounded || ignoreGrounded)
            {
                var newVel = velocity;
                newVel.y += overrideImpulse ?? playerStats.JumpImpulse;
                velocity = newVel;
            }
        }
    }
}

