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
       [SerializeField] private InputActionReference mouseDelta;
       [SerializeField] private InputActionReference jump;
       
       private Dictionary<string, InputActionReference> _allActions = new();    
       public Vector2 Direction => move.action.ReadValue<Vector2>();
       public Vector2 MouseDelta => mouseDelta.action.ReadValue<Vector2>();
       public NetworkBool JumpStatus { get; private set; } = false;
       
       public event Action<bool> Jump;
       
       public void EnableInputActions()
       {
           _allActions.TryAdd(move.action.name, move);
           _allActions.TryAdd(mouseDelta.action.name, mouseDelta);
           _allActions.TryAdd(jump.action.name, jump);
           
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
       }
       
       private void RemoveListeners()
       {
           jump.action.performed -= OnJumpPerformed;
           jump.action.canceled -= OnJumpCanceled;
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
   } 
}

