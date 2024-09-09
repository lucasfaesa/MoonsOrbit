using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LocalPlayer
{
   [CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/Player/InputReader")]
   public class InputReaderSO : ScriptableObject
   {
       [Header("Input Actions")]
       [SerializeField] private InputActionReference move;
       [SerializeField] private InputActionReference look;
       [SerializeField] private InputActionReference jump;
       [SerializeField] private InputActionReference shoot;
       [SerializeField] private InputActionReference aim;
       
       
       private Dictionary<string, InputActionReference> _allActions = new();    
       public Vector2 Direction => move.action.ReadValue<Vector2>();
       public Vector2 MouseDelta => look.action.ReadValue<Vector2>();
       public NetworkBool JumpStatus { get; private set; } = false;
       public NetworkBool ShootStatus { get; private set; } = false;
       public NetworkBool AimStatus { get; private set; } = false;
       
       public event Action<bool> Jump;
       public event Action<bool> Shoot;
       public event Action<bool> Aim;
       
       public void EnableInputActions()
       {
           _allActions.TryAdd(move.action.name, move);
           _allActions.TryAdd(look.action.name, look);
           _allActions.TryAdd(jump.action.name, jump);
           _allActions.TryAdd(shoot.action.name, shoot);
           _allActions.TryAdd(aim.action.name, aim);
           
           foreach (var actionRef in _allActions.Values)
               actionRef.action.Enable();
           
           AddListeners();
       }
       
       public void DisableInputActions()
       {
           foreach (var actionRef in _allActions.Values)
               actionRef.action.Disable();
           
           RemoveListeners();
       }
       
       private void AddListeners()
       {
           jump.action.performed += OnJumpPerformed;
           jump.action.canceled += OnJumpCanceled;
           
           shoot.action.performed += OnShootPerformed;
           shoot.action.canceled += OnShootCanceled;
           
           aim.action.performed += OnAimPerformed;
           aim.action.canceled += OnAimCanceled;
           
       }
       
       private void RemoveListeners()
       {
           jump.action.performed -= OnJumpPerformed;
           jump.action.canceled -= OnJumpCanceled;
           
           shoot.action.performed -= OnShootPerformed;
           shoot.action.canceled -= OnShootCanceled;
           
           aim.action.performed -= OnAimPerformed;
           aim.action.canceled -= OnAimCanceled;
       }
   
       private void OnJumpPerformed(InputAction.CallbackContext callbackContext)
       {
           Jump?.Invoke(true);
           JumpStatus = true;
       }
   
       private void OnJumpCanceled(InputAction.CallbackContext callbackContext)
       {
           Jump?.Invoke(false);
           JumpStatus = false;
       }
       
       private void OnShootPerformed(InputAction.CallbackContext callbackContext)
       {
           Shoot?.Invoke(true);
           ShootStatus = true;
       }
   
       private void OnShootCanceled(InputAction.CallbackContext callbackContext)
       {
           Shoot?.Invoke(false);
           ShootStatus = false;
       }
       
       private void OnAimPerformed(InputAction.CallbackContext callbackContext)
       {
           Aim?.Invoke(true);
           AimStatus = true;
       }
   
       private void OnAimCanceled(InputAction.CallbackContext callbackContext)
       {
           Aim?.Invoke(false);
           AimStatus = false;
       }
   } 
}

