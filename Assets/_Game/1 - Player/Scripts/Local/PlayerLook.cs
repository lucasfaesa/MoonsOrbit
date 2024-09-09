using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalPlayer
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("SOs")]
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private PlayerStatsSO playerStats;
        [Header("Other")]
        [SerializeField] private Camera playerCamera;
        
        private float _verticalLookRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleLook();
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

