using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class LocalPlayerToPuppetSynchronizer : MonoBehaviour
    {
        [Header("SOs")]
        [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
        [Header("Components")]
        [SerializeField] private Transform playerModelTransform;
    
    
        private void OnEnable()
        {
            networkRunnerCallbacks.Input += OnInput;
        }

        private void OnDisable()
        {
            networkRunnerCallbacks.Input -= OnInput;
        }

        private void OnInput(NetworkRunner networkRunner, NetworkInput networkInput)
        {
            HardwareRigInputData inputData = new HardwareRigInputData();
            
            inputData.PlayerModelVisualPos = playerModelTransform.position;
            inputData.PlayerModelVisualRot = playerModelTransform.rotation;

            networkInput.Set(inputData);
        }
    }
}

public struct HardwareRigInputData : INetworkInput
{
    public Vector3 PlayerModelVisualPos { get; set; }
    public Quaternion PlayerModelVisualRot { get; set; }
    
}

