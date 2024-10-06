using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class PuppetPlayerMovement : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private GameObject playerVisuals;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!GetInput<PuppetPlayerInputData>(out var inputData))
                return;
        
            this.transform.SetPositionAndRotation(inputData.PlayerTransformNetworkData.PlayerModelVisualPos, inputData.PlayerTransformNetworkData.PlayerModelVisualRot);
        }
        
        
    }
}

