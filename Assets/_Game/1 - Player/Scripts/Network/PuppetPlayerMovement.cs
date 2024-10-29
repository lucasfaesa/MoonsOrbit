using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class PuppetPlayerMovement : NetworkBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform yawTransform;
        [SerializeField] private Transform pitchTransform;
        
        
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!GetInput<PuppetPlayerInputData>(out var inputData))
                return;
        
            this.transform.position =  inputData.PlayerTransformNetworkData.PlayerModelVisualPos;
            
            yawTransform.rotation = inputData.PlayerTransformNetworkData.PlayerModelVisualRot;
            pitchTransform.localRotation = inputData.PlayerTransformNetworkData.PlayerCameraLocalRot;
        }
        
        
    }
}

