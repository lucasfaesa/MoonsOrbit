using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("SOs")] 
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [SerializeField] private InputReaderSO inputReader;
    
    private void OnEnable()
    {
        networkRunnerCallbacks.Input += OnInput;
    }

    private void OnDisable()
    {
        networkRunnerCallbacks.Input -= OnInput;
        inputReader.DisableInputActions();
    }

    private void Start()
    {
        inputReader.EnableInputActions();
    }

    private void OnInput(NetworkRunner networkRunner, NetworkInput networkInput)
    {
        PlayerInputData inputData = new PlayerInputData();
        
        Vector3 direction = new Vector3(inputReader.Direction.x, 0, inputReader.Direction.y); 
        
        inputData.Direction = direction;

        networkInput.Set(inputData);
    }
}

public struct PlayerInputData : INetworkInput
{
    public Vector3 Direction;
}