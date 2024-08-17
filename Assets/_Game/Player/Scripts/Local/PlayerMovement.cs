using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalPlayer
{
    public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private CharacterController characterController;

    [Header("Other")]
    [SerializeField] private Camera playerCamera;
    
    private float _verticalLookRotation;
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

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        var direction = inputReader.Direction;
        Vector3 move = playerCamera.transform.right * direction.x 
                       + playerCamera.transform.forward * direction.y;
        
        MoveWithoutRotation(move);
    }
    
    private void MoveWithoutRotation(Vector3 direction) {
        float deltaTime = Time.fixedDeltaTime;
        float tickRate = 1f / Time.fixedDeltaTime; // Assuming you want to match the fixed update rate.
        Vector3 previousPos = transform.position;
        Vector3 moveVelocity = velocity;

        direction = direction.normalized;

        if (grounded && moveVelocity.y < 0) {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += playerStats.Gravity * deltaTime;

        Vector3 horizontalVel = new Vector3();
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;

        if (direction == Vector3.zero) {
            horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, playerStats.Braking * deltaTime);
        } else {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * playerStats.Acceleration * deltaTime, playerStats.MaxSpeed);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        characterController.Move(moveVelocity * deltaTime);

        velocity = (transform.position - previousPos) * tickRate;
        grounded = characterController.isGrounded;
    }

    private void OnJump(bool jumpPressed)
    {
        if(jumpPressed && grounded)
            Jump();
    }
    
    private void Jump(bool ignoreGrounded = false, float? overrideImpulse = null) {
        if (grounded || ignoreGrounded) {
            var newVel = velocity;
            newVel.y += overrideImpulse ?? playerStats.JumpImpulse;
            velocity =  newVel;
        }
    }

    private void HandleLook()
    {
        var mouseDelta = inputReader.MouseDelta;
        
        
        transform.Rotate(0, mouseDelta.x * Time.deltaTime * playerStats.MouseSensitivity.x, 0);
        
        float mouseY = mouseDelta.y * playerStats.MouseSensitivity.y * Time.deltaTime;
        
        _verticalLookRotation -= mouseY;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(_verticalLookRotation, 0, 0);

    }
}
}

