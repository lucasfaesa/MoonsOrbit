using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/Player/InputReader")]
public class InputReaderSO : ScriptableObject
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference move;
    
    private Dictionary<string, InputActionReference> _allActions = new();    
    public Vector3 Direction => move.action.ReadValue<Vector2>();
    
    public void EnableInputActions()
    {
        _allActions.TryAdd(move.action.name, move);
        
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
    }
    
    private void RemoveListeners()
    {
    }
}
