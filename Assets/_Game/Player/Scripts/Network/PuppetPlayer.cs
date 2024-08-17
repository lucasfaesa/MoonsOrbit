using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Networking
{
    public class PuppetPlayer : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private NetworkTransform remotePlayerTransform;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private GameObject playerVisuals;

        public bool IsLocalNetworkRig => Object.HasStateAuthority;
    
        public override void Spawned()
        {
            base.Spawned();

            if (IsLocalNetworkRig)
            {
                capsuleCollider.gameObject.SetActive(false);
                playerVisuals.gameObject.SetActive(false);
            }
            else
                Debug.Log("This is a client object");
            
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!GetInput<HardwareRigInputData>(out var inputData))
                return;
        
            remotePlayerTransform.transform.SetPositionAndRotation(inputData.PlayerModelVisualPos, inputData.PlayerModelVisualRot);
        }
    
    }
}

